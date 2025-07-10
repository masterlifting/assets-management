using AM.Portfolio.Worker.Background.Tasks;

using Microsoft.Extensions.Options;

using Net.Shared.Background.Abstractions;
using Net.Shared.Background.Models;
using Net.Shared.Models.Settings;
using Net.Shared.Persistence.Abstractions.Repositories.Sql;

namespace AM.Portfolio.Worker.Background.Services;

public sealed class EventsBackgroundService : Net.Shared.Background.Core.BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EventsBackgroundService> _logger;

    public EventsBackgroundService(
        IServiceScopeFactory scopeFactory,
        IBackgroundServiceConfigurationProvider provider,
        ILogger<EventsBackgroundService> logger) : base(EventsBackgroundTask.Name, provider, logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task Run(BackgroundTaskModel taskModel, CancellationToken cToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        #region GET DEPENDENCIES
        var processRepository = scope.ServiceProvider.GetRequiredService<IPersistenceSqlProcessRepository>();
        var hostOptions = scope.ServiceProvider.GetRequiredService<IOptions<HostSettings>>();
        #endregion

        var task = new EventsBackgroundTask(_logger, hostOptions, processRepository);

        await task.Run(taskModel, cToken);
    }
}
