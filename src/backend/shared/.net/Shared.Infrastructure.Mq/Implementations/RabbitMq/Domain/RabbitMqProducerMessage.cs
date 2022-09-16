using Shared.Infrastructure.Mq.Implementations.RabbitMq.Enums;
using Shared.Infrastructure.Mq.Interfaces;

namespace Shared.Infrastructure.Mq.Implementations.RabbitMq.Domain;

public class RabbitMqProducerMessage : IMqProducerMessage<IMqPayload>
{
    public string Id { get; set; } = null!;
    public IMqPayload Payload { get; set; } = null!;
    public IDictionary<string, string> Headers { get; init; } = null!;
    
    public IMqQueue Queue { get; set; } = null!;
    public RabbitMqExchangeNames Exchange { get; set; }
    public IMqProducerMessageSettings? Settings { get; set; }

    public string Version { get; set; } = null!;
}