using Microsoft.Extensions.DependencyInjection;

using Shared.Background.Settings;
using Shared.Persistense.Abstractions.Entities.EntityState;

namespace Shared.Background.Abstractions.EntityState;

public sealed class EntityStateBackgroundTask<TEntity> : IBackgroundTask where TEntity : class, IEntityState
{
    public string Name { get; }

    private readonly IServiceScopeFactory _scopeFactory;

    public EntityStateBackgroundTask(IServiceScopeFactory scopeFactory)
    {
        Name = typeof(TEntity).Name + "Task";
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(int count, BackgroundTaskSettings settings, CancellationToken cToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;

        var pipeline = serviceProvider.GetRequiredService<EntityStateProcessing<TEntity>>();

        await pipeline.StartAsync(Name, count, settings, cToken);
    }
}