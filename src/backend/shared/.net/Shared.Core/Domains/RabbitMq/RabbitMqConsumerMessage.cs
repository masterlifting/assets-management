using Shared.Core.Abstractions.Queues;

namespace Shared.Core.Domains.RabbitMq;

public class RabbitMqConsumerMessage : IMqConsumerMessage<string>
{
    public string Payload { get; set; } = null!;
    public IDictionary<string, string> Headers { get; init; } = null!;

    public DateTime DateTime { get; init; } = DateTime.UtcNow;

    public string Exchange { get; set; } = null!;
    public string RoutingKey { get; set; } = null!;
    public string ConsumerTag { get; set; } = null!;
    public ulong DeliveryTag { get; set; }
}