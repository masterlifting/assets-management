using Shared.Queue.Abstractions.Domain;
using Shared.Queue.Abstractions.Settings;

namespace Shared.Queue.Abstractions.Connection;

public interface IMqConsumer : IDisposable
{
    void Consume<TPayload>(IMqConsumerSettings settings, CancellationToken cToken, Func<IMqConsumerSettings, IReadOnlyCollection<IMqConsumerMessage<TPayload>>, CancellationToken, Task> func)
        where TPayload : class;
    Task HandleMessagesAsync<TPayload>(IMqConsumerSettings settings, CancellationToken cToken, Func<IMqConsumerSettings, IReadOnlyCollection<IMqConsumerMessage<TPayload>>, CancellationToken, Task> func)
        where TPayload : class;
}