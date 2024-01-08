using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Bcs.Companies;
using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Bcs.Transactions;
using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Raiffeisen.Serbia.Transactions;
using AM.Portfolio.Worker.Background.Tasks;

using Microsoft.Extensions.Options;

using Net.Shared.Background.Abstractions;
using Net.Shared.Background.Models;
using Net.Shared.Models.Settings;
using Net.Shared.Persistence.Abstractions.Repositories.NoSql;

namespace AM.Portfolio.Worker.Background.Services;

public sealed class DataHeapBackgroundService : Net.Shared.Background.Core.BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DataHeapBackgroundTask> _logger;

    public DataHeapBackgroundService(
        IServiceScopeFactory scopeFactory,
        IBackgroundServiceConfigurationProvider provider,
        ILogger<DataHeapBackgroundTask> logger) : base(DataHeapBackgroundTask.Name, provider, logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task Run(BackgroundTaskModel taskModel, CancellationToken cToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        #region GET DEPENDENCIES
        var bcsCompaniesHandler = scope.ServiceProvider.GetRequiredService<IBcsCompaniesHandler>();
        var bcsTransactionsHandler = scope.ServiceProvider.GetRequiredService<IBcsTransactionsHandler>();
        var raiffeisenSrbTransactionsHandler = scope.ServiceProvider.GetRequiredService<IRaiffeisenSrbTransactionsHandler>();

        var processRepository = scope.ServiceProvider.GetRequiredService<IPersistenceNoSqlProcessRepository>();
        var hostOptions = scope.ServiceProvider.GetRequiredService<IOptions<HostSettings>>();
        #endregion

        var task = new DataHeapBackgroundTask(
            _logger
            , hostOptions
            , processRepository
            , bcsCompaniesHandler
            , bcsTransactionsHandler
            , raiffeisenSrbTransactionsHandler);

        await task.Run(taskModel, cToken);
    }
}
