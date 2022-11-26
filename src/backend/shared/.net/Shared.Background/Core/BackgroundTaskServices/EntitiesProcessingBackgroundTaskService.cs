using Microsoft.Extensions.DependencyInjection;
using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Base;
using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace Shared.Background.Core.BackgroundTaskServices;

internal sealed class EntitiesProcessingBackgroundTaskService<TEntity, TStep> : BackgroundTaskServiceBase<TEntity, TStep, EntitiesProcessingBackgroundTask<TEntity, TStep>>
    where TEntity : class, IProcessableEntity
    where TStep : class, IProcessableEntityStep
{
    internal EntitiesProcessingBackgroundTaskService(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
    }
}