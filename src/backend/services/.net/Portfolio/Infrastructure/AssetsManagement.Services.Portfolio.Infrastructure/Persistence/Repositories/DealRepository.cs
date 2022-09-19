using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Infrastructure.Persistense.Abstractions.Entities.State.Handle;
using Shared.Infrastructure.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public class DealRepository<TContext> : EntityStateRepository<Deal, TContext>, IDealRepository
    where TContext : DbContext, IEntityStateDbContext
{
    protected DealRepository(ILogger<Deal> logger, TContext context) : base(logger, context)
    {
    }

    public override Task CreateRangeAsync(IReadOnlyCollection<Deal> entities, CancellationToken? ctToken = null)
    {
        foreach (var item in entities)
        {
            item.StateId = (int) Shared.Infrastructure.Persistense.Enums.States.Ready; 
            item.StepId = (int)Core.Domain.Persistense.Entities.Enums.Steps.Calculating;
        }

        return base.CreateRangeAsync(entities, ctToken);
    }
    public Task<Deal[]> GetFullDealsAsync(IEnumerable<Derivative> derivatives)
    {
        throw new NotImplementedException();
    }
}