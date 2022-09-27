using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public sealed class UserRepository<TContext> : Repository<User, TContext>, IUserRepository
    where TContext : DbContext
{
    private readonly TContext _context;

    public UserRepository(ILogger<User> logger, TContext context) : base(logger, context)
    {
        _context = context;
    }

    public ValueTask<User?> FindAsync(string userId) => _context.Set<User>().FindAsync(userId);
}