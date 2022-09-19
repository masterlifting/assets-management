using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;
using AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Infrastructure.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public class AccountRepository<TContext> : Repository<Account, TContext> , IAccountRepository
    where TContext : DbContext
{
    protected AccountRepository(ILogger<Account> logger, TContext context) : base(logger, context)
    {
    }

    public Task<Dictionary<string, int>> GetGroupedAccountsByProviderAsync(ProviderId providerId, CancellationToken cToken) => DbSet
        .Where(x => x.ProviderId == providerId.AsInt)
        .Select(x => ValueTuple.Create(x.Name, x.Id))
        .ToDictionaryAsync(x => x.Item1, x => x.Item2, cToken);
}