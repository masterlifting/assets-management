namespace Shared.Infrastructure.Mq.Interfaces;

public interface IMqProducerMessage<TPayload> : IMqMessage<TPayload> where TPayload : class
{
    IMqQueue Queue { get; set; }
    IMqProducerMessageSettings? Settings { get; set; }
    string Version { get; set; }
}