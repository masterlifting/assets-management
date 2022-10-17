using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

using Microsoft.Extensions.DependencyInjection;

using Shared.Background.Abstractions.Tasks;
using Shared.Background.Handling.EntityState;
using Shared.Background.Settings;

using System.Threading;
using System.Threading.Tasks;

namespace AM.Services.Portfolio.Host.Services.Background.EntityState.Tasks;

public sealed class DerivativeBackgroundTask : IEntityStateBackgroundTask
{
    private readonly IServiceScopeFactory _scopeFactory;
    public string Name { get; }
    public DerivativeBackgroundTask(string taskName, IServiceScopeFactory scopeFactory)
    {
        Name = taskName;
        _scopeFactory = scopeFactory;
    }
    public async Task StartAsync(int count, BackgroundTaskSettings settings, CancellationToken cToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;

        var pipeline = serviceProvider.GetRequiredService<EntityStateHandling<Derivative, IDerivativeRepository>>();

        await pipeline.StartAsync(count, settings, cToken);
    }
}