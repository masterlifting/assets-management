using Shared.Queue.Domain.RabbitMq;

namespace Shared.Queue.Settings.RabbitMq;

public sealed class RabbitMqModelBuilderSettings
{
    public RabbitMqExchange Exchange { get; set; } = null!;
    public RabbitMqQueue Queue { get; set; } = null!;
}