namespace Shared.MessagesQueue.Interfaces;

public interface IMqConsumerSettings
{
    int Limit { get; set; }
    string Queue { get; set; }
}