using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Context;
using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public sealed class DerivativeRepository<TContext> : SqlProcessableEntityRepository<Derivative, TContext>, IDerivativeRepository
    where TContext : DbContext, IProcessableDbContext
{
    private readonly TContext _context;

    public DerivativeRepository(ILogger<Derivative> logger, TContext context) : base(logger, context)
    {
        _context = context;
    }

    public async Task<Dictionary<string, string[]>> GetGroupedDerivativesAsync(CancellationToken cToken)
    {
        var derivatives = await _context.Set<Derivative>().Select(x => ValueTuple.Create(x.Id, x.Code)).ToArrayAsync(cToken);

        return derivatives
            .GroupBy(x => x.Item1)
            .ToDictionary(x => x.Key, x => x.Select(y => y.Item2).ToArray());
    }
}