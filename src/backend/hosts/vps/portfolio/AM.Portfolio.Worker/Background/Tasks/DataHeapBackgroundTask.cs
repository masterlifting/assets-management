using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Bcs.Companies;
using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Bcs.Transactions;
using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Raiffeisen.Serbia.Transactions;
using AM.Portfolio.Core.Persistence.Entities.NoSql;
using AM.Portfolio.Core.Persistence.Entities.NoSql.Catalogs;
using AM.Portfolio.Worker.Background.Tasks.StepHandlers;

using Microsoft.Extensions.Options;

using Net.Shared.Background.Abstractions;
using Net.Shared.Background.Core;
using Net.Shared.Models.Settings;
using Net.Shared.Persistence.Abstractions.Entities.Catalogs;
using Net.Shared.Persistence.Abstractions.Repositories.NoSql;

namespace AM.Portfolio.Worker.Background.Tasks;

public sealed class DataHeapBackgroundTask : BackgroundTask<DataHeap>
{
    public const string Name = "HandleDataHeap";

    private readonly HostSettings _hostSettings;
    private readonly IPersistenceNoSqlProcessRepository _processRepository;

    private readonly IBcsCompaniesHandler _bcsCompaniesHandler;
    private readonly IBcsTransactionsHandler _bcsTransactionsHandler;
    private readonly IRaiffeisenSrbTransactionsHandler _raiffeisenSrbTransactionsHandler;

    public DataHeapBackgroundTask(
        ILogger logger,
        IOptions<HostSettings> hostOptions,
        IPersistenceNoSqlProcessRepository processRepository,
        IBcsCompaniesHandler bcsCompaniesHandler,
        IBcsTransactionsHandler bcsTransactionsHandler,
        IRaiffeisenSrbTransactionsHandler raiffeisenSrbTransactionsHandler) : base(logger)
    {
        _hostSettings = hostOptions.Value;
        _bcsCompaniesHandler = bcsCompaniesHandler;
        _bcsTransactionsHandler = bcsTransactionsHandler;
        _raiffeisenSrbTransactionsHandler = raiffeisenSrbTransactionsHandler;
        _processRepository = processRepository;
    }

    protected override IBackgroundTaskStep<DataHeap> RegisterStepHandler() =>
        new DataHeapStepHandler(
            _bcsCompaniesHandler
            , _bcsTransactionsHandler
            , _raiffeisenSrbTransactionsHandler);
    protected override async Task<IPersistentProcessStep[]> GetSteps(CancellationToken cToken) =>
        await _processRepository.GetProcessSteps<ProcessSteps>(cToken);
    protected override async Task<DataHeap[]> GetProcessableData(IPersistentProcessStep step, int limit, CancellationToken cToken) =>
        await _processRepository.GetProcessableData<DataHeap>(_hostSettings.Id, step, limit, cToken);
    protected override async Task<DataHeap[]> GetUnprocessedData(IPersistentProcessStep step, int limit, DateTime updateTime, int maxAttempts, CancellationToken cToken) =>
        await _processRepository.GetUnprocessedData<DataHeap>(_hostSettings.Id, step, limit, updateTime, maxAttempts, cToken);
    protected override async Task SaveData(IPersistentProcessStep currentStep, IPersistentProcessStep? nextStep, IEnumerable<DataHeap> data, CancellationToken cToken) =>
        await _processRepository.SetProcessedData(_hostSettings.Id, currentStep, nextStep, data, cToken);
}
