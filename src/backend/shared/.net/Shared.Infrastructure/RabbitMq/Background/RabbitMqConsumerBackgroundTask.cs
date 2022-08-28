using Shared.Core.Abstractions.Background.MessageQueue;
using Shared.Core.Abstractions.Queues;
using Shared.Core.Domains.RabbitMq;

namespace Shared.Infrastructure.RabbitMq.Background;

public abstract class RabbitMqConsumerBackgroundTask : IMqConsumerBackgroundTask
{
    public abstract string Name { get; }

    public Task StartAsync<TPayload>(IReadOnlyCollection<IMqConsumerMessage<TPayload>> messages, CancellationToken cToken)
        where TPayload : class =>
        StartAsync((IReadOnlyCollection<RabbitMqConsumerMessage>)messages, cToken);

    protected abstract Task StartAsync(IReadOnlyCollection<RabbitMqConsumerMessage> messages, CancellationToken cToken);
}