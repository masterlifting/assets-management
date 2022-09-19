using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities.State.Handle;
using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public class DerivativeRepository<TContext> : EntityStateRepository<Derivative, TContext>, IDerivativeRepository
    where TContext : DbContext, IEntityStateDbContext
{
    protected DerivativeRepository(ILogger<Derivative> logger, TContext context) : base(logger, context)
    {
    }

    public async Task<Dictionary<string, string[]>> GetGroupedDerivativesAsync(CancellationToken cToken)
    {
        var derivatives = await DbSet.Select(x => ValueTuple.Create(x.Id, x.Code)).ToArrayAsync(cToken);

        return derivatives
            .GroupBy(x => x.Item1)
            .ToDictionary(x => x.Key, x => x.Select(y => y.Item2).ToArray());
    }
}