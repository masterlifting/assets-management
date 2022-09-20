using Shared.MessagesQueue.Domain.RabbitMq.Connection;

namespace Shared.MessagesQueue.Settings.RabbitMq;

public class RabbitMqSection
{
    public const string Name = "RabbitMq";
    public RabbitMqClient Client { get; set; } = null!;
    public Dictionary<string, RabbitMqConsumerSettings>? Consumers { get; set; }
}