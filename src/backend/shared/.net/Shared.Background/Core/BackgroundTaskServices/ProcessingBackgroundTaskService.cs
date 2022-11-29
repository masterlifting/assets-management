using Microsoft.Extensions.DependencyInjection;
using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Base;
using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace Shared.Background.Core.BackgroundTaskServices;

internal sealed class ProcessingBackgroundTaskService<TEntity, TStep> : BackgroundTaskServiceBase<TEntity, TStep, ProcessingBackgroundTask<TEntity, TStep>>
    where TEntity : class, IPersistensableProcess
    where TStep : class, IProcessableStep
{
    internal ProcessingBackgroundTaskService(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
    }
}