using Shared.Contracts.Settings;
using Shared.Infrastructure.Mq.Interfaces;

namespace Shared.Infrastructure.Mq.Implementations.RabbitMq.Domain;

public class RabbitMqConsumerSettings : IMqConsumerSettings
{
    public string Queue { get; set; } = null!;
    public int Limit { get; set; }
    public SchedulerSettings Scheduler { get; set; } = new();

    public bool IsExclusiveQueue { get; set; }
    public bool IsAutoAck { get; set; }
    public string? ConsumerTag { get; set; }
    public bool IsNoLocal { get; set; }

    public IDictionary<string, object>? Arguments { get; set; }
}