using Microsoft.Extensions.DependencyInjection;
using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Base;
using Shared.Persistense.Abstractions.Entities;

namespace Shared.Background.Core.BackgroundTaskServices;

internal sealed class SynchronousRequestsBackgroundTaskService<T> : BackgroundTaskServiceBase<T, SynchronousRequestsBackgroundTask<T>>
    where T : class, IEntityProcessable
{
    internal SynchronousRequestsBackgroundTaskService(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
    }
}