using Shared.Infrastructure.Mq.Implementations.RabbitMq.Enums;

namespace Shared.Infrastructure.Mq.Implementations.RabbitMq.Domain;

public class RabbitMqExchange
{
    public RabbitMqExchangeTypes Type { get; set; }
    public RabbitMqExchangeNames Name { get; set; }
    public bool IsDurable { get; set; }
    public bool IsAutoDelete { get; set; }
    public IDictionary<string, object>?  Arguments { get; set; }
}