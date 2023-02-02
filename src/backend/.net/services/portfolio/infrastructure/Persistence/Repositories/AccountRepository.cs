using AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;

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

        public Task<Account> GetAccountAsync(string agreement, Guid userId, int providerId)
        {
            throw new NotImplementedException();
        }
    }
}
