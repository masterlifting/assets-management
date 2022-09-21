namespace Shared.MessagesQueue.Abstractions.Settings
{
    public interface IMqConsumerSettings
    {
        int Limit { get; set; }
        string Queue { get; set; }
    }
}