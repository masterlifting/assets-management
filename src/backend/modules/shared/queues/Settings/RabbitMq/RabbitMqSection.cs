using Shared.Queue.Domain.RabbitMq.Connection;

namespace Shared.Queue.Settings.RabbitMq;

public sealed class RabbitMqSection
{
    public const string Name = "RabbitMq";
    public RabbitMqClient Client { get; set; } = null!;
    public Dictionary<string, RabbitMqConsumerSettings>? Consumers { get; set; }
}