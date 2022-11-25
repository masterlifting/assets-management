using Microsoft.Extensions.DependencyInjection;
using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Base;
using Shared.Persistense.Abstractions.Entities;

namespace Shared.Background.Core.BackgroundTaskServices;

internal sealed class EntitiesProcessingBackgroundTaskService<T> : BackgroundTaskServiceBase<T, EntitiesProcessingBackgroundTask<T>>
    where T : class, IEntityProcessable
{
    internal EntitiesProcessingBackgroundTaskService(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
    }
}