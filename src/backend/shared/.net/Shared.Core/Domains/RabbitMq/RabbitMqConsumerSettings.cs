using Shared.Contracts.Settings;
using Shared.Core.Abstractions.Queues;

namespace Shared.Core.Domains.RabbitMq;

public class RabbitMqConsumerSettings : IMqConsumerSettings
{
    public string QueueName { get; set; } = null!;

    public int Limit { get; set; }
    public SchedulerSettings Scheduler { get; set; } = new();

    public bool IsExclusiveQueue { get; set; }
    public bool IsAutoAck { get; set; }
    public string? ConsumerTag { get; set; }
    public bool IsNoLocal { get; set; }

    public IDictionary<string, object>? Arguments { get; set; }
}