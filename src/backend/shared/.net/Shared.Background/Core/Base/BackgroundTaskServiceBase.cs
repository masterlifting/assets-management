using Microsoft.Extensions.DependencyInjection;

using Shared.Background.Interfaces;
using Shared.Background.Settings;
using Shared.Persistense.Abstractions.Entities;

namespace Shared.Background.Core.Base;

public abstract class BackgroundTaskServiceBase<TEntity, TTask> : IBackgroundTaskService
    where TEntity : class, IEntityProcessable
    where TTask : BackgroundTaskBase
{
    public string TaskName { get; }

    private readonly IServiceScopeFactory _scopeFactory;

    protected BackgroundTaskServiceBase(IServiceScopeFactory scopeFactory)
    {
        TaskName = typeof(TEntity).Name + '.' + typeof(TTask).Name;
        _scopeFactory = scopeFactory;
    }

    public async Task RunTaskAsync(int taskCount, BackgroundTaskSettings settings, CancellationToken cToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;

        var task = serviceProvider.GetRequiredService<TTask>();

        await task.StartAsync(TaskName, taskCount, settings, cToken);
    }
}