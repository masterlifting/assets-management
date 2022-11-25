using Shared.Background.Settings.Models;

namespace Shared.Background.Settings;

public sealed class BackgroundTaskSettings
{
    public EntitiesProcessingStepsSettings Steps { get; set; } = new();
    public BackgroundTaskSchedulerSettings Scheduler { get; set; } = new();
    public BackgroundTaskRetryPolicySettings? RetryPolicy { get; set; };
}