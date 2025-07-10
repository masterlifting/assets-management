using AM.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs.Transactions;
using AM.Portfolio.Core.Persistence.Entities.Sql;

using Net.Shared.Persistence.Abstractions.Repositories.Sql;
using Net.Shared.Persistence.Models.Contexts;

namespace AM.Portfolio.Infrastructure.Persistence.Repositories;

public sealed class DealRepository : IDealRepository
{
    private readonly IPersistenceSqlReaderRepository _readerRepository;

    public DealRepository(IPersistenceSqlReaderRepository readerRepository)
    {
        _readerRepository = readerRepository;
    }

    public Task<DateTime[]> GetDealDates(Account account, BcsTransactionsResult report, CancellationToken cToken)
    {
        var incomeOptions = new PersistenceSelectorOptions<Income, DateTime>
        {
            QueryOptions = new()
            {
                Filter = x =>
                    x.AccountId == account.Id
                    && x.HolderId == account.HolderId
                    && x.DateTime >= report.DateStart
                    && x.DateTime <= report.DateEnd,
            },

            Selector = x => x.DateTime
        };

        return _readerRepository.FindMany(incomeOptions, cToken);
    }
}
