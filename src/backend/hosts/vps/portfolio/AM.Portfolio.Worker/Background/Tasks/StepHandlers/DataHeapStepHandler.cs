using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Bcs.Companies;
using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Bcs.Transactions;
using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Raiffeisen.Serbia.Transactions;
using AM.Portfolio.Core.Persistence.Entities.NoSql;

using Net.Shared.Background.Abstractions;
using Net.Shared.Models.Domain;
using Net.Shared.Persistence.Abstractions.Entities.Catalogs;

using static AM.Portfolio.Core.Constants.Enums;

namespace AM.Portfolio.Worker.Background.Tasks.StepHandlers;

public sealed class DataHeapStepHandler : IBackgroundTaskStep<DataHeap>
{
    private readonly IBcsCompaniesHandler _bcsCompaniesHandler;
    private readonly IBcsTransactionsHandler _bcsTransactionsHandler;
    private readonly IRaiffeisenSrbTransactionsHandler _raiffeisenSrbTransactionsHandler;

    public DataHeapStepHandler(
        IBcsCompaniesHandler bcsCompaniesHandler
        , IBcsTransactionsHandler bcsTransactionsHandler
        , IRaiffeisenSrbTransactionsHandler raiffeisenSrbTransactionsHandler)
    {
        _bcsCompaniesHandler = bcsCompaniesHandler;
        _bcsTransactionsHandler = bcsTransactionsHandler;
        _raiffeisenSrbTransactionsHandler = raiffeisenSrbTransactionsHandler;
    }

    public async Task<Result<DataHeap>> Handle(IPersistentProcessStep step, IEnumerable<DataHeap> data, CancellationToken cToken)
    {
        switch (step.Id)
        {
            case (int)ProcessSteps.ParseBcsCompanies:
                {
                    await _bcsCompaniesHandler.Handle(data, cToken);

                    return new(data);
                }
            case (int)ProcessSteps.ParseBcsTransactions:
                {
                    await _bcsTransactionsHandler.Handle(data, cToken);

                    return new(data);
                }
            case (int)ProcessSteps.ParseRaiffeisenSrbTransactions:
                {
                    await _raiffeisenSrbTransactionsHandler.Handle(data, cToken);

                    return new(data);
                }
            default:
                throw new NotImplementedException($"The step '{step.Name}' of the task '{DataHeapBackgroundTask.Name}' was not recognized.");
        }
    }
}
