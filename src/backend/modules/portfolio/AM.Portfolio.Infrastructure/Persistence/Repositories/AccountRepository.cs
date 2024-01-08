using AM.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Portfolio.Core.Persistence.Entities.Sql;
using AM.Portfolio.Infrastructure.Exceptions;

using Net.Shared.Persistence.Abstractions.Repositories.Sql;
using Net.Shared.Persistence.Models.Contexts;

namespace AM.Portfolio.Infrastructure.Persistence.Repositories;

public sealed class AccountRepository : IAccountRepository
{
    private readonly IPersistenceSqlReaderRepository _readerRepository;

    public AccountRepository(IPersistenceSqlReaderRepository readerRepository)
    {
        _readerRepository = readerRepository;
    }

    public async Task<Account> Get(string agreement, int holderId, CancellationToken cToken)
    {
        var options = new PersistenceQueryOptions<Account>
        {
            Filter = x => 
                x.HolderId == holderId 
                && x.Agreement == agreement,
        };

        var account = await _readerRepository.FindSingle(options, cToken);

        return account ?? throw new PortfolioInfrastructureException($"Account for the agreement '{agreement}' was not found");
    }
}
