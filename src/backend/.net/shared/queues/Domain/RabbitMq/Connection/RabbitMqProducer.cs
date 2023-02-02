using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

using Shared.Extensions.Logging;
using Shared.Extensions.Serialization;
using Shared.Queue.Abstractions.Connection;
using Shared.Queue.Abstractions.Domain;
using Shared.Queue.Exceptions;
using Shared.Queue.Settings.RabbitMq;

using System.Text;

namespace Shared.Queue.Domain.RabbitMq.Connection;

public sealed class RabbitMqProducer : IMqProducer
{
    private readonly string _initiator;

    private readonly ILogger<RabbitMqProducer> _logger;
    private readonly RabbitMqClient _client;
    private IModel? _model;

    public RabbitMqProducer(ILogger<RabbitMqProducer> logger, RabbitMqClient client)
    {
        _logger = logger;
        _client = client;

        var objectId = GetHashCode();
        _initiator = $"RabbitMq Producer {objectId}";
    }

    public bool TryPublish<TPayload>(IMqProducerMessage<TPayload> message, out string error) where TPayload : class
    {
        error = string.Empty;

        try
        {
            Publish((RabbitMqProducerMessage)message);

            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(new SharedQueueException(nameof(RabbitMqProducer), Constants.Actions.Post, new(exception)));

            return false;
        }
    }
    public void Publish<TPayload>(IMqProducerMessage<TPayload> message) where TPayload : class
    {
        Publish((RabbitMqProducerMessage)message);
    }

    public void Dispose()
    {
        _model?.Close();
        _model?.Dispose();

        _logger.LogDebug(_initiator, Constants.Actions.Disconnect, Constants.Actions.Success);
    }

    private void Publish(RabbitMqProducerMessage message)
    {
        _model ??= _client.CreateModel();

        _logger.LogTrace(_initiator, Constants.Actions.Post, Constants.Actions.Start, message.Id);

        var exchangeName = string.Intern($"{message.Exchange}");
        _model.BasicPublish(
            exchangeName
            , $"{exchangeName}.{message.Queue.Name}"
            , new BasicProperties(message)
            , Encoding.UTF8.GetBytes(message.Payload.Serialize()));

        _logger.LogTrace(_initiator, Constants.Actions.Post, Constants.Actions.Success, message.Id);
    }
    private class BasicProperties : IBasicProperties
    {
        private readonly RabbitMqProducerMessageSettings _settings;

        public BasicProperties(RabbitMqProducerMessage message)
        {
            Headers = message.Headers.ToDictionary(x => x.Key, y => y.Value as object);
            Headers.Add("VERSION", message.Version);
            _settings = message.Settings as RabbitMqProducerMessageSettings ?? new RabbitMqProducerMessageSettings();
        }

        public ushort ProtocolClassId => 0;
        public string? ProtocolClassName => null;

        public IDictionary<string, object> Headers { get; set; }

        public string? AppId { get; set; }
        public string? ClusterId { get; set; }
        public string? ContentEncoding { get; set; }
        public string? ContentType { get; set; }
        public string? CorrelationId { get; set; }
        public byte DeliveryMode { get; set; }
        public string? Expiration { get; set; }
        public string? MessageId { get; set; }
        public bool Persistent { get; set; }
        public byte Priority { get; set; }
        public string? ReplyTo { get; set; }
        public PublicationAddress? ReplyToAddress { get; set; }
        public AmqpTimestamp Timestamp { get; set; }
        public string? Type { get; set; }
        public string? UserId { get; set; }


        public void ClearAppId() => throw new NotImplementedException();
        public void ClearClusterId() => throw new NotImplementedException();
        public void ClearContentEncoding() => throw new NotImplementedException();
        public void ClearContentType() => throw new NotImplementedException();
        public void ClearCorrelationId() => throw new NotImplementedException();
        public void ClearDeliveryMode() => throw new NotImplementedException();
        public void ClearExpiration() => throw new NotImplementedException();
        public void ClearHeaders() => throw new NotImplementedException();
        public void ClearMessageId() => throw new NotImplementedException();
        public void ClearPriority() => throw new NotImplementedException();
        public void ClearReplyTo() => throw new NotImplementedException();
        public void ClearTimestamp() => throw new NotImplementedException();
        public void ClearType() => throw new NotImplementedException();
        public void ClearUserId() => throw new NotImplementedException();
        public bool IsAppIdPresent() => throw new NotImplementedException();
        public bool IsClusterIdPresent() => throw new NotImplementedException();
        public bool IsContentEncodingPresent() => throw new NotImplementedException();
        public bool IsContentTypePresent() => throw new NotImplementedException();
        public bool IsCorrelationIdPresent() => throw new NotImplementedException();
        public bool IsDeliveryModePresent() => throw new NotImplementedException();
        public bool IsExpirationPresent() => throw new NotImplementedException();
        public bool IsHeadersPresent() => throw new NotImplementedException();
        public bool IsMessageIdPresent() => throw new NotImplementedException();
        public bool IsPriorityPresent() => throw new NotImplementedException();
        public bool IsReplyToPresent() => throw new NotImplementedException();
        public bool IsTimestampPresent() => throw new NotImplementedException();
        public bool IsTypePresent() => throw new NotImplementedException();
        public bool IsUserIdPresent() => throw new NotImplementedException();
    }
}
