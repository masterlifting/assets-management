﻿namespace Shared.Background.Settings.Sections;

public class BackgroundTaskSection
{
    public const string Name = "Background";
    public Dictionary<string, BackgroundTaskSettings>? Tasks { get; set; }
}