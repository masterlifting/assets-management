using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public sealed class AccountRepository<TContext> : Repository<Account, TContext>, IAccountRepository
    where TContext : DbContext
{
    private readonly TContext _context;

    public AccountRepository(ILogger<Account> logger, TContext context) : base(logger, context)
    {
        _context = context;
    }

    public Task<Dictionary<string, int>> GetGroupedAccountsByProviderAsync(ProviderId providerId, CancellationToken cToken) => 
        _context.Set<Account>()
            .Where(x => x.ProviderId == providerId.AsInt)
            .Select(x => ValueTuple.Create(x.Name, x.Id))
            .ToDictionaryAsync(x => x.Item1, x => x.Item2, cToken);
}