using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public class UserRepository<TContext> : Repository<User, TContext> , IUserRepository
    where TContext : DbContext
{
    protected UserRepository(ILogger<User> logger, TContext context) : base(logger, context)
    {
    }
}