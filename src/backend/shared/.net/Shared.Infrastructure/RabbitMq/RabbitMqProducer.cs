using Microsoft.Extensions.Logging;

using RabbitMQ.Client;
using Shared.Core.Constants;
using Shared.Core.Extensions;

using System.Text;
using Shared.Core.Abstractions.Queues;
using Shared.Core.Domains.RabbitMq;

namespace Shared.Infrastructure.RabbitMq;

public sealed class RabbitMqProducer : IMqProducer
{
    private readonly string _initiator;

    private readonly ILogger<RabbitMqProducer> _logger;
    private readonly IModel _model;

    public RabbitMqProducer(ILogger<RabbitMqProducer> logger, RabbitMqClient client)
    {
        _logger = logger;

        var objectId = GetHashCode();
        _initiator = $"RabbitMQ Producer {objectId}";

        _model = client.CreateModel();
    }

    public bool TryPublish<TPayload>(IMqProducerMessage<TPayload> message, out string error) where TPayload : class
    {
        error = string.Empty;
        var info = string.Intern($"To queue {message.Queue.Name}");

        try
        {
            _logger.LogTrace(_initiator, CoreConstants.Actions.Send, CoreConstants.Results.Start, info);

            PublishToRabbitMq((RabbitMqProducerMessage)message);

            _logger.LogTrace(_initiator, CoreConstants.Actions.Send, CoreConstants.Results.Success, info);

            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(_initiator, CoreConstants.Actions.Send, exception);

            return false;
        }
    }
    public void Publish<TPayload>(IMqProducerMessage<TPayload> message) where TPayload : class
    {
        var info = string.Intern($"To queue {message.Queue.Name}");

        _logger.LogTrace(_initiator, CoreConstants.Actions.Send, CoreConstants.Results.Start, info);

        PublishToRabbitMq((RabbitMqProducerMessage)message);

        _logger.LogTrace(_initiator, CoreConstants.Actions.Send, CoreConstants.Results.Success, info);
    }
    public void Dispose()
    {
        _logger.LogDebug(_initiator, CoreConstants.Actions.Disconnect, CoreConstants.Results.Start);

        _model.Close();
        _model.Dispose();

        _logger.LogDebug(_initiator, CoreConstants.Actions.Disconnect, CoreConstants.Results.Success);
    }

    private void PublishToRabbitMq(RabbitMqProducerMessage message)
    {
        var exchangeName = string.Intern($"{message.Exchange}");
        _model.BasicPublish(
            exchangeName
            , $"{exchangeName}.{message.Queue}"
            , new BasicProperties(message.Headers, (RabbitMqProducerMessageSettings?)message.Settings)
            , Encoding.UTF8.GetBytes(JsonSerializerExtensions.Serialize(message.Payload)));
    }
    private class BasicProperties : IBasicProperties
    {
        private readonly RabbitMqProducerMessageSettings _settings;

        public BasicProperties(IDictionary<string, string> headers, RabbitMqProducerMessageSettings? settings)
        {
            Headers = headers.ToDictionary(x => x.Key, y => y.Value as object);

            if (settings != null)
            {
                _settings = settings;
                Headers.Add(TransferConstants.Version, settings.Version);
            }

            _settings = new();
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