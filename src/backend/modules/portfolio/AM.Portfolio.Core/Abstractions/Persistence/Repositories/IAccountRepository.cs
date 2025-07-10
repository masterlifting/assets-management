using AM.Portfolio.Core.Persistence.Entities.Sql;

namespace AM.Portfolio.Core.Abstractions.Persistence.Repositories;

public interface IAccountRepository
{
    Task<Account> Get(string agreement, int holderId, CancellationToken cToken);
}
