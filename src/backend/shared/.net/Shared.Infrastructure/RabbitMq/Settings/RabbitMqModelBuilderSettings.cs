using Shared.Core.Domains.RabbitMq;

namespace Shared.Infrastructure.RabbitMq.Settings;

public class RabbitMqModelBuilderSettings
{
    public RabbitMqExchange Exchange { get; set; } = null!;
    public RabbitMqQueue Queue { get; set; } = null!;
}