namespace Shared.Infrastructure.Background.Settings.Sections;

public class BackgroundTaskSection
{
    public const string SectionName = "Background";
    public Dictionary<string, BackgroundTaskSettings>? Tasks { get; set; }
}