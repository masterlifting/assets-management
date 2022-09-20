using Shared.MessagesQueue.Domain.RabbitMq;

namespace Shared.MessagesQueue.Settings.RabbitMq;

public class RabbitMqModelBuilderSettings
{
    public RabbitMqExchange Exchange { get; set; } = null!;
    public RabbitMqQueue Queue { get; set; } = null!;
}