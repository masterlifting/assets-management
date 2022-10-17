using Shared.MessagesQueue.Abstractions.Domain;
using Shared.MessagesQueue.Abstractions.Settings;

namespace Shared.MessagesQueue.Abstractions.Connection;

public interface IMqConsumer : IDisposable
{
    void Consume<TPayload>(IMqConsumerSettings settings, CancellationToken cToken, Func<IMqConsumerSettings, IReadOnlyCollection<IMqConsumerMessage<TPayload>>, CancellationToken, Task> func)
        where TPayload : class;
    Task HandleMessagesAsync<TPayload>(IMqConsumerSettings settings, CancellationToken cToken, Func<IMqConsumerSettings, IReadOnlyCollection<IMqConsumerMessage<TPayload>>, CancellationToken, Task> func)
        where TPayload : class;
}