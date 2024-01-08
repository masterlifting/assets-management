using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Raiffeisen.Serbia.Transactions;
using AM.Portfolio.Core.Models.Services.DataHeapServices.Raiffeisen.Serbia.Transactions;
using AM.Portfolio.Core.Persistence.Entities.Sql;

using static AM.Portfolio.Core.Constants.Enums;

namespace AM.Portfolio.Core.Services.DataHeapServices.Raiffeisen.Serbia.Transactions;

public class RaiffeisenSrbTransactionsMapper : IRaiffeisenSrbTransactionsMapper
{
    private static int HolderId => (int)Holders.RaiffeisenSerbia;

    public IReadOnlyCollection<Deal> Deals { get; } = Array.Empty<Deal>();
    public IReadOnlyCollection<Event> Events { get; } = Array.Empty<Event>();

    public Task Map(RaiffeisenSrbTransactionsResult result, CancellationToken cToken)
    {
        throw new NotImplementedException();
    }
}
