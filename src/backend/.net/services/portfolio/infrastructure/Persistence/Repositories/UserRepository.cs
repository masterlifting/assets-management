using AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Infrastructure.Persistence.Contexts;

using Microsoft.Extensions.Logging;

using Shared.Persistence.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories
{
    public sealed class UserRepository : PostgreRepository<User, PostgrePortfolioContext>, IUserRepository
    {
        public UserRepository(ILogger<User> logger, PostgrePortfolioContext context) : base(logger, context)
        {
        }
    }
}
