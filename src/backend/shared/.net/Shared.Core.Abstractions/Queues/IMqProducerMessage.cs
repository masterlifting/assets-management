namespace Shared.Core.Abstractions.Queues;

public interface IMqProducerMessage<TPayload> : IMqMessage<TPayload> where TPayload : class
{
    IMqQueue Queue { get; set; }
    IMqProducerMessageSettings? Settings { get; set; }
}