namespace Shared.MessagesQueue.Interfaces;

public interface IMqConsumerMessage<TPayload> : IMqMessage<TPayload> where TPayload : class
{
    DateTime DateTime { get; init; }
}