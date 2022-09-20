using Shared.MessagesQueue.Abstractions.Domain;

namespace Shared.MessagesQueue.Domain.RabbitMq;

public sealed class RabbitMqQueue : IMqQueue
{
    public string Name { get; set; } = null!;
    public bool IsDurable { get; set; }
    public bool IsExclusive { get; set; }
    public bool IsAutoDelete { get; set; }
    public IDictionary<string, object>? Arguments { get; set; }
}