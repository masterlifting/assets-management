using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Context;
using Shared.Persistense.Repositories;

using static Shared.Persistense.Constants.Enums;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public sealed class DealRepository<TContext> : SqlProcessableEntityRepository<Deal, TContext>, IDealRepository
    where TContext : DbContext, IProcessableDbContext
{
    public DealRepository(ILogger<Deal> logger, TContext context) : base(logger, context)
    {
    }

    public override Task CreateRangeAsync(IReadOnlyCollection<Deal> entities, CancellationToken? cToken = null)
    {
        foreach (var item in entities)
        {
            item.StatusId = (int)Statuses.Ready;
            item.StepId = (int)Steps.Computing;
        }

        return base.CreateRangeAsync(entities, cToken);
    }
    public Task<Deal[]> GetFullDealsAsync(IEnumerable<Derivative> derivatives)
    {
        throw new NotImplementedException();
    }
}