using AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Infrastructure.Persistence.Contexts;

using Microsoft.Extensions.Logging;

using Shared.Persistence.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories
{
    public sealed class AccountRepository : PostgreRepository<Account, PostgrePortfolioContext>, IAccountRepository
    {
        public AccountRepository(ILogger<Account> logger, PostgrePortfolioContext context) : base(logger, context)
        {
        }

        public Task<Account> GetAccountAsync(string agreement, Guid userId, int providerId)
        {
            throw new NotImplementedException();
        }
    }
}
