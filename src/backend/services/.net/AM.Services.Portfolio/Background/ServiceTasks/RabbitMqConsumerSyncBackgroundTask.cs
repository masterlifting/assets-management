using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Domains.RabbitMq;
using Shared.Infrastructure.RabbitMq.Background;

namespace AM.Services.Portfolio.Background.ServiceTasks;

public class RabbitMqConsumerSyncBackgroundTask : RabbitMqConsumerBackgroundTask
{
    public override string Name => "RabbitMQ синхронизация";
    
    private readonly IServiceScopeFactory _scopeFactory;
    public RabbitMqConsumerSyncBackgroundTask(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    protected override Task StartAsync(IReadOnlyCollection<RabbitMqConsumerMessage> messages, CancellationToken cToken)
    {
        throw new System.NotImplementedException();
    }
}