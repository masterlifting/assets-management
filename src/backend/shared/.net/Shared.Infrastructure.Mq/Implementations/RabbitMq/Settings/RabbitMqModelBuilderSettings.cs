using Shared.Infrastructure.Mq.Implementations.RabbitMq.Domain;

namespace Shared.Infrastructure.Mq.Implementations.RabbitMq.Settings;

public class RabbitMqModelBuilderSettings
{
    public RabbitMqExchange Exchange { get; set; } = null!;
    public RabbitMqQueue Queue { get; set; } = null!;
}