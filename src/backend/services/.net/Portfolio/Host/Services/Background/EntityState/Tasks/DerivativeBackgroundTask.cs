using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Enums;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

using Microsoft.Extensions.DependencyInjection;

using Shared.Infrastructure.Background.Abstractions.EntityState;
using Shared.Infrastructure.Background.Settings;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shared.Infrastructure.Persistense.Handlers;

namespace AM.Services.Portfolio.Host.Services.Background.EntityState.Tasks;

public class DerivativeBackgroundTask : IEntityStateBackgroundTask
{
    private readonly IServiceScopeFactory _scopeFactory;
    public string Name { get; }
    public DerivativeBackgroundTask(string taskName, IServiceScopeFactory scopeFactory)
    {
        Name = taskName;
        _scopeFactory = scopeFactory;
    }
    public async Task StartAsync(BackgroundTaskSettings settings, CancellationToken cToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var pipeline = scope.ServiceProvider.GetRequiredService<EntityStatePipeline<Derivative>>();

        var steps = new Queue<int>(new[]
        {
            (int)Steps.Calculating
        });

        await pipeline.StartAsync(settings, steps, cToken);
    }
}