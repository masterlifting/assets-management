using Microsoft.Extensions.Logging;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Core.Constants;
using System.Text;
using Shared.Core.Abstractions.Queues;
using Shared.Core.Domains.RabbitMq;
using Shared.Core.Exceptions;
using static Shared.Core.Extensions.LoggerExtensions;

namespace Shared.Infrastructure.RabbitMq;

public sealed class RabbitMqConsumer : IMqConsumer
{
    private readonly SemaphoreSlim _semaphore = new(1);

    private readonly string _initiator;
    private readonly List<RabbitMqConsumerMessage> _messages;

    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly RabbitMqClient _client;

    private IModel? _model;

    public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger, RabbitMqClient client)
    {
        _logger = logger;
        _client = client;

        _messages = new(1);

        var objectId = GetHashCode();
        _initiator = $"RabbitMQ Consumer {objectId}";
    }

    public async Task ConsumeAsync<TPayload>(IMqConsumerSettings settings, Func<IReadOnlyCollection<IMqConsumerMessage<TPayload>>, CancellationToken, Task> func, CancellationToken cToken)
        where TPayload : class
    {
        var rabbitMqSettings = settings as RabbitMqConsumerSettings ?? throw new Exception($"Ошибка конфигурации {nameof(RabbitMqConsumerSettings)}");

        await _semaphore.WaitAsync(cToken).ConfigureAwait(false);

        if (_messages.Count > 0)
            await HandleMessagesAsync(func, (int)settings.Limit, cToken);

        _semaphore.Release();

        ConsumeToRabbitMq(rabbitMqSettings, OnReceivedAsync);

        async Task OnReceivedAsync(object? _, BasicDeliverEventArgs args)
        {
            await _semaphore.WaitAsync(cToken).ConfigureAwait(false);

            _messages.Add(GetMessage(args));

            if (_messages.Count == settings.Limit)
                await HandleMessagesAsync(func, (int)settings.Limit, cToken);

            _semaphore.Release();
        }
    }

    public void Stop()
    {
        _logger.LogDebug(_initiator, CoreConstants.Actions.Disconnect, CoreConstants.Results.Start);

        _model?.Close();
        _model?.Dispose();
        _model = null;

        _logger.LogDebug(_initiator, CoreConstants.Actions.Disconnect, CoreConstants.Results.Success);
    }

    private void ConsumeToRabbitMq(RabbitMqConsumerSettings settings, AsyncEventHandler<BasicDeliverEventArgs> onReceivedAsync)
    {
        if (_model is not null)
            return;

        _model = _client.CreateModel();

        var consumer = new AsyncEventingBasicConsumer(_model);
        consumer.Received += onReceivedAsync;

        _model.BasicConsume(
            settings.QueueName,
            settings.IsAutoAck,
            settings.ConsumerTag,
            settings.IsNoLocal,
            settings.IsExclusiveQueue,
            settings.Arguments,
            consumer);
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
    private async Task HandleMessagesAsync<TPayload>(
        Func<IReadOnlyCollection<IMqConsumerMessage<TPayload>>, CancellationToken, Task> func
        , int limit
        , CancellationToken cToken)
    where TPayload : class
    {
        try
        {
            _logger.LogTrace(_initiator, CoreConstants.Actions.HandleItems, CoreConstants.Results.Start);

            await func.Invoke(
                _messages.AsReadOnly() as IReadOnlyCollection<IMqConsumerMessage<TPayload>> 
                  ?? throw new SharedCoreNotCastException(_initiator, CoreConstants.Actions.HandleItem, "No Cast")
                , cToken);

            _logger.LogDebug(_initiator, CoreConstants.Actions.HandleItems, CoreConstants.Results.Success);
        }
        catch (Exception exception)
        {
            _logger.LogError(_initiator, CoreConstants.Actions.HandleItems, exception);
        }
        finally
        {
            _messages.Clear();

            if (limit > _messages.Capacity)
                _messages.Capacity = limit;
        }
    }
}