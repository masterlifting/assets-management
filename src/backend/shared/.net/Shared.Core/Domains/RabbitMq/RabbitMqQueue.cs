using Shared.Core.Abstractions.Queues;

namespace Shared.Core.Domains.RabbitMq;

public class RabbitMqQueue : IMqQueue
{
    public string Name { get; set; } = null!;
    public bool IsDurable { get; set; }
    public bool IsExclusive { get; set; }
    public bool IsAutoDelete { get; set; }
    public IDictionary<string, object>?  Arguments { get; set; }
}