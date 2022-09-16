namespace Shared.Infrastructure.Mq.Interfaces;

public interface IMqConsumer : IDisposable
{
    void Consume<TPayload>(IMqConsumerSettings settings, CancellationToken cToken, Func<IMqConsumerSettings, IReadOnlyCollection<IMqConsumerMessage<TPayload>>, CancellationToken, Task> func) 
        where TPayload : class;
    Task HandleMessagesAsync<TPayload>(IMqConsumerSettings settings, CancellationToken cToken, Func<IMqConsumerSettings, IReadOnlyCollection<IMqConsumerMessage<TPayload>>, CancellationToken, Task> func) 
        where TPayload : class;
}