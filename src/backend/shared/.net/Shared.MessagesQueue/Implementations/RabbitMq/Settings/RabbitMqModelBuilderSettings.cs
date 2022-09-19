using Shared.MessagesQueue.Implementations.RabbitMq.Domain;

namespace Shared.MessagesQueue.Implementations.RabbitMq.Settings;

public class RabbitMqModelBuilderSettings
{
    public RabbitMqExchange Exchange { get; set; } = null!;
    public RabbitMqQueue Queue { get; set; } = null!;
}