namespace Shared.Contracts.Settings;

public class BackgroundTaskSettings
{
    public int Limit { get; set; }
    public SchedulerSettings Scheduler { get; set; } = new ();
    public RetryTaskSettings? Retry { get; set; }
}