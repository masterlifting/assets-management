using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Domains.RabbitMq;
using Shared.Infrastructure.RabbitMq.Background;

namespace AM.Services.Portfolio.Background.ServiceTasks;

public class RabbitMqConsumerIncomingBackgroundTask : RabbitMqConsumerBackgroundTask
{
    public override string Name => "RabbitMq прием входящих";
    
    private readonly IServiceScopeFactory _scopeFactory;
    public RabbitMqConsumerIncomingBackgroundTask(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    protected override Task StartAsync(IReadOnlyCollection<RabbitMqConsumerMessage> messages, CancellationToken cToken)
    {
        throw new System.NotImplementedException();
    }
}