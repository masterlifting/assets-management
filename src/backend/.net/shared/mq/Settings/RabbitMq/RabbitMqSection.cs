using Shared.MQ.Domain.RabbitMq.Connection;

namespace Shared.MQ.Settings.RabbitMq;

public sealed class RabbitMqSection
{
    public const string Name = "RabbitMq";
    public RabbitMqClient Client { get; set; } = null!;
    public Dictionary<string, RabbitMqConsumerSettings>? Consumers { get; set; }
}