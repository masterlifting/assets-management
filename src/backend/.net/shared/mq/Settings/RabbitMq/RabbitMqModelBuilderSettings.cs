using Shared.MQ.Domain.RabbitMq;

namespace Shared.MQ.Settings.RabbitMq;

public sealed class RabbitMqModelBuilderSettings
{
    public RabbitMqExchange Exchange { get; set; } = null!;
    public RabbitMqQueue Queue { get; set; } = null!;
}