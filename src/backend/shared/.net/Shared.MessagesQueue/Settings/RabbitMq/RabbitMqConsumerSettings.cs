using Shared.Background.Settings.Models;
using Shared.MessagesQueue.Abstractions.Settings;

namespace Shared.MessagesQueue.Settings.RabbitMq;

public sealed class RabbitMqConsumerSettings : IMqConsumerSettings
{
    public string Queue { get; set; } = null!;
    public int Limit { get; set; }
    public BackgroundTaskSchedulerSettings Scheduler { get; set; } = new();

    public bool IsExclusiveQueue { get; set; }
    public bool IsAutoAck { get; set; }
    public string? ConsumerTag { get; set; }
    public bool IsNoLocal { get; set; }

    public IDictionary<string, object>? Arguments { get; set; }
}