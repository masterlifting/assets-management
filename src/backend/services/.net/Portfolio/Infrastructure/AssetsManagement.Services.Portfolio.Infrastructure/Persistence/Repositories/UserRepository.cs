using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Infrastructure.Persistense.Repositories.Implementation;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public class UserRepository<TContext> : Repository<User, TContext> , IUserRepository
    where TContext : DbContext
{
    protected UserRepository(ILogger<User> logger, TContext context) : base(logger, context)
    {
    }
}