using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Shared.Background.Core.BackgroundTaskServices;
using Shared.Background.Core.Base;
using Shared.Background.Settings.Sections;
using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace Shared.Background.Core.BackgroundServices;

public abstract class ProcessingBackgroundService<TEntity, TStep> : BackgroundServiceBase<TEntity> 
    where TEntity : class, IProcessableEntity
    where TStep : class, IProcessableEntityStep
{
    protected ProcessingBackgroundService(
        IOptionsMonitor<BackgroundTaskSection> options
        , ILogger logger
        , IServiceScopeFactory scopeFactory) : base(options, logger, new ProcessingBackgroundTaskService<TEntity, TStep>(scopeFactory))
    {
    }
}