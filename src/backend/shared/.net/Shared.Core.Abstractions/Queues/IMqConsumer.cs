namespace Shared.Core.Abstractions.Queues;

public interface IMqConsumer
{
    Task ConsumeAsync<TPayload>(IMqConsumerSettings settings, Func<IReadOnlyCollection<IMqConsumerMessage<TPayload>>, CancellationToken, Task> func, CancellationToken cToken) 
        where TPayload : class;
    void Stop();
}