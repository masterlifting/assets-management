namespace Shared.Background.Settings;

public class RetryTaskSettings
{
    public int Minutes { get; set; } = 30;
    public int MaxAttempts { get; set; } = 10;
}