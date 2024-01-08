using AM.Services.Portfolio.Core.Domain.Persistence.Entities;

using Shared.Persistence.Abstractions.Repositories;

namespace AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories
{
    public interface IAccountRepository : IPersistenceSqlRepository<Account>
    {
        Task<Account> GetAccountAsync(string agreement, Guid userId, int providerId);
    }
}
