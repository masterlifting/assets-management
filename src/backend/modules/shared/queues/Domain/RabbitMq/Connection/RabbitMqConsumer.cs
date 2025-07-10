using Microsoft.Extensions.Logging;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Shared.Extensions.Logging;

using System.Text;
using Shared.Queue.Abstractions.Connection;
using Shared.Queue.Settings.RabbitMq;
using Shared.Queue.Abstractions.Settings;
using Shared.Queue.Abstractions.Domain;
using Shared.Queue.Exceptions;

namespace Shared.Queue.Domain.RabbitMq.Connection;

public sealed class RabbitMqConsumer : IMqConsumer
{
    private readonly SemaphoreSlim _semaphore = new(1);

    private readonly string _initiator;
    private readonly List<RabbitMqConsumerMessage> _messages;

    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly RabbitMqClient _client;

    public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger, RabbitMqClient client)
    {
        _logger = logger;
        _client = client;

        _messages = new(Constants.Limit);

        var objectId = GetHashCode();
        _initiator = $"RabbitMq consumer {objectId}";
    }

    public void Consume<TPayload>(IMqConsumerSettings settings, CancellationToken cToken, Func<IMqConsumerSettings, IReadOnlyCollection<IMqConsumerMessage<TPayload>>, CancellationToken, Task> func) where TPayload : class
    {
        var consumerSettings = settings as RabbitMqConsumerSettings ?? throw new Exception($"Configeration error from: '{nameof(RabbitMqConsumerSettings)}'");

        using var model = _client.CreateModel();

        var consumer = new AsyncEventingBasicConsumer(model);
        consumer.Received += OnReceivedAsync;

        model.BasicConsume(
            consumerSettings.Queue,
            consumerSettings.IsAutoAck,
            consumerSettings.ConsumerTag,
            consumerSettings.IsNoLocal,
            consumerSettings.IsExclusiveQueue,
            consumerSettings.Arguments,
            consumer);

        async Task OnReceivedAsync(object? _, BasicDeliverEventArgs args)
        {
            var message = GetMessage(args);

            await _semaphore.WaitAsync(cToken);

            _messages.Add(message);

            if (_messages.Count == settings.Limit)
                await InvokeRunAsync(settings, cToken, func);

            _semaphore.Release();
        }
    }

    public async Task HandleMessagesAsync<TPayload>(IMqConsumerSettings settings, CancellationToken cToken, Func<IMqConsumerSettings, IReadOnlyCollection<IMqConsumerMessage<TPayload>>, CancellationToken, Task> func) where TPayload : class
    {
        await _semaphore.WaitAsync(cToken);

        if (_messages.Count > 0)
            await InvokeRunAsync(settings, cToken, func);

        _semaphore.Release();
    }
    public void Dispose()
    {
        _logger.LogDebug(_initiator, Constants.Actions.Disconnect, Constants.Actions.Success);
    }

    private Task InvokeRunAsync<TPayload>(IMqConsumerSettings settings, CancellationToken cToken, Func<IMqConsumerSettings, IReadOnlyCollection<IMqConsumerMessage<TPayload>>, CancellationToken, Task> func)
        where TPayload : class
    {
        try
        {
            return func.Invoke(settings, (IReadOnlyCollection<IMqConsumerMessage<TPayload>>)_messages.AsReadOnly(), cToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(new SharedQueueException(_initiator, "Processing incoming messages", new(exception)));
        }
        finally
        {
            _messages.Clear();
        }

        return Task.CompletedTask;
    }

    private static Dictionary<string, string> GetHeaders(IBasicProperties properties)
    {
        var headers = new Dictionary<string, string>(properties.Headers.Count, StringComparer.OrdinalIgnoreCase);

        foreach (var key in properties.Headers.Keys)
        {
            var value = properties.Headers[key]?.ToString();

            if (string.IsNullOrEmpty(value))
                continue;

            headers.Add(key, value);
        }

        return headers;
    }
    private static RabbitMqConsumerMessage GetMessage(BasicDeliverEventArgs args) => new()
    {
        Payload = Encoding.UTF8.GetString(args.Body.ToArray()),
        Headers = GetHeaders(args.BasicProperties),
        Exchange = args.Exchange,
        RoutingKey = args.RoutingKey,
        ConsumerTag = args.ConsumerTag,
        DeliveryTag = args.DeliveryTag
    };
}
