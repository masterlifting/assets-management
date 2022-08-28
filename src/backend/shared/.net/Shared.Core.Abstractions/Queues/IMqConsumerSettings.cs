using Shared.Contracts.Settings;

namespace Shared.Core.Abstractions.Queues;

public interface IMqConsumerSettings
{
    int Limit { get; set; }
    string QueueName { get; set; }
    SchedulerSettings Scheduler { get; set; }
}