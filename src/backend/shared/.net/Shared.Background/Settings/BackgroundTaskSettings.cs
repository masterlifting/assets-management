using static Shared.Persistense.Constants.Enums;

namespace Shared.Background.Settings;

public sealed class BackgroundTaskSettings
{
    public int Limit { get; set; }
    public SchedulerSettings Scheduler { get; set; } = new();
    public RetryTaskSettings Retry { get; set; } = new();
    public Steps[] Steps { get; set; } = Array.Empty<Steps>();
}