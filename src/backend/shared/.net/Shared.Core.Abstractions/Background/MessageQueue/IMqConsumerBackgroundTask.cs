using Shared.Core.Abstractions.Queues;

namespace Shared.Core.Abstractions.Background.MessageQueue;

public interface IMqConsumerBackgroundTask : IBackgroundTask
{
    Task StartAsync<TPayload>(IReadOnlyCollection<IMqConsumerMessage<TPayload>> messages, CancellationToken cToken)
        where TPayload : class;
}