﻿namespace Shared.Background.Settings.Models;

public sealed class BackgroundTaskRetryPolicySettings
{
    public int EveryTime { get; set; } = 5;
    public int MaxAttempts { get; set; } = 10;
}