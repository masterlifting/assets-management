using AM.Services.Common.Contracts.Background;
using static AM.Services.Market.Enums;

namespace AM.Services.Market.Settings.Sections;

public class LoadTaskSettings : BackgroundTaskSettings
{
    public Sources[] Sources { get; set; } = Array.Empty<Sources>();
    public int DaysAgo { get; set; } = 1;
}