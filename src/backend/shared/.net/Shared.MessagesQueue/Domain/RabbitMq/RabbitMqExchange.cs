using static Shared.MessagesQueue.Constants.Enums.RabbitMq;

namespace Shared.MessagesQueue.Domain.RabbitMq;

public class RabbitMqExchange
{
    public ExchangeTypes Type { get; set; }
    public ExchangeNames Name { get; set; }
    public bool IsDurable { get; set; }
    public bool IsAutoDelete { get; set; }
    public IDictionary<string, object>? Arguments { get; set; }
}