using Shared.Core.Abstractions.Queues;

namespace Shared.Core.Domains.RabbitMq;

public class RabbitMqProducerMessage : IMqProducerMessage<IMqPayload>
{
    public IMqPayload Payload { get; set; } = null!;
    public IDictionary<string, string> Headers { get; init; } = null!;
    
    public IMqQueue Queue { get; set; } = null!;
    public IMqProducerMessageSettings? Settings { get; set; }

    public RabbitMqExchangeNames Exchange { get; set; }
}