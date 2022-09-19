using Shared.MessagesQueue.Implementations.RabbitMq.Domain;

namespace Shared.MessagesQueue.Implementations.RabbitMq.Settings.Sections;

public class RabbitMqSection
{
    public const string Name = "RabbitMq";
    public RabbitMqClient Client { get; set; } = null!;
    public Dictionary<string, RabbitMqConsumerSettings>? Consumers { get; set; }
}