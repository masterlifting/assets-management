using AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Infrastructure.Exceptions;

using Microsoft.Extensions.Logging;
using Shared.Persistence.Abstractions.Contexts;
using Shared.Persistence.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories
{
    public sealed class AccountRepository : PostgreRepository<Account>, IAccountRepository
    {
        private readonly IPostgrePersistenceContext _context;

        public AccountRepository(ILogger<Account> logger, IPostgrePersistenceContext context) : base(logger, context)
        {
            _context = context;
        }

        public async Task<Account> GetAccountAsync(string agreement, Guid userId, int providerId)
        {
            var account = await _context.FindSingleAsync<Account>(x => x.UserId == userId && x.ProviderId == providerId && x.Name == agreement);
            return account ?? throw new PortfolioInfrastructureException(nameof(AccountRepository), nameof(GetAccountAsync), new($"Account for the agreement '{agreement}' was not found"));
        }
    }
}
