using AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs.Transactions;
using AM.Portfolio.Core.Persistence.Entities.Sql;

namespace AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Bcs.Transactions;

public interface IBcsTransactionsMapper : IDataHeapMapper<BcsTransactionsResult>
{
    IReadOnlyCollection<Deal> Deals { get; }
    IReadOnlyCollection<Event> Events { get; }
}
