using Shared.MQ.Abstractions.Domain;

namespace Shared.MQ.Abstractions.Connection;

public interface IMqProducer : IDisposable
{
    bool TryPublish<TPayload>(IMqProducerMessage<TPayload> message, out string error) where TPayload : class;
    void Publish<TPayload>(IMqProducerMessage<TPayload> message) where TPayload : class;
}