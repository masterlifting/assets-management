﻿namespace AM.Services.Market.Settings.Sections;

public class ComputeDataSettings
{
    public ComputeTaskSettings[] Tasks { get; set; } = Array.Empty<ComputeTaskSettings>();
}