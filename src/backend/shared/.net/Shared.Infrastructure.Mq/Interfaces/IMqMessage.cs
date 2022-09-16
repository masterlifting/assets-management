namespace Shared.Infrastructure.Mq.Interfaces;

public interface IMqMessage<T> where T : class
{
    T Payload { get; set; }
    IDictionary<string, string> Headers { get; init; }
}