using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Shared.Background.Core.BackgroundTaskServices;
using Shared.Background.Core.Base;
using Shared.Background.Settings.Sections;
using Shared.Persistense.Abstractions.Entities;

namespace Shared.Background.Core.BackgroundServices;

public abstract class SynchronousRequestsBackgroundService<T> : BackgroundServiceBase<T> where T : class, IEntityProcessable
{
    protected SynchronousRequestsBackgroundService(
        IOptionsMonitor<BackgroundTaskSection> options
        , ILogger logger
        , IServiceScopeFactory scopeFactory) : base(options, logger, new SynchronousRequestsBackgroundTaskService<T>(scopeFactory))
    {
    }
}