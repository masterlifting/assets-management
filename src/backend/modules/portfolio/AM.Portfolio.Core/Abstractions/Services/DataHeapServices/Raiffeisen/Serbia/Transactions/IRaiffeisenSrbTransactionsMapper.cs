using AM.Portfolio.Core.Models.Services.DataHeapServices.Raiffeisen.Serbia.Transactions;
using AM.Portfolio.Core.Persistence.Entities.Sql;

namespace AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Raiffeisen.Serbia.Transactions;

public interface IRaiffeisenSrbTransactionsMapper : IDataHeapMapper<RaiffeisenSrbTransactionsResult>
{
    IReadOnlyCollection<Deal> Deals { get; }
    IReadOnlyCollection<Event> Events { get; }
}
