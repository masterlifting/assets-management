using Shared.MQ.Abstractions.Domain;
using Shared.MQ.Abstractions.Settings;

namespace Shared.MQ.Abstractions.Connection;

public interface IMqConsumer : IDisposable
{
    void Consume<TPayload>(IMqConsumerSettings settings, CancellationToken cToken, Func<IMqConsumerSettings, IReadOnlyCollection<IMqConsumerMessage<TPayload>>, CancellationToken, Task> func)
        where TPayload : class;
    Task HandleMessagesAsync<TPayload>(IMqConsumerSettings settings, CancellationToken cToken, Func<IMqConsumerSettings, IReadOnlyCollection<IMqConsumerMessage<TPayload>>, CancellationToken, Task> func)
        where TPayload : class;
}