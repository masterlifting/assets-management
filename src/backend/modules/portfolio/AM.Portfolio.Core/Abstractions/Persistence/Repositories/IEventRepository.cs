using AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs.Transactions;
using AM.Portfolio.Core.Persistence.Entities.Sql;

namespace AM.Portfolio.Core.Abstractions.Persistence.Repositories;

public interface IEventRepository
{
    Task<DateTime[]> GetEventDates(Account account, BcsTransactionsResult result, CancellationToken cToken);
}
