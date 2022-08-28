namespace Shared.Contracts.Settings;

public class RetryTaskSettings
{
    public int Minutes { get; set; } = 10;
    public int MaxAttempts { get; set; } = 10;
}