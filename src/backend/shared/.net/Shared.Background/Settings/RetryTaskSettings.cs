namespace Shared.Background.Settings;

public class RetryTaskSettings
{
    public int Skip { get; set; } = 5;
    public int Attempts { get; set; } = 10;
}