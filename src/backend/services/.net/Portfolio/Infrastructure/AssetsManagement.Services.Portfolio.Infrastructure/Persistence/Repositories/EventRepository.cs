using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Infrastructure.Persistense.Entities.EntityState;
using Shared.Infrastructure.Persistense.Repositories.Implementation;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public class EventRepository<TContext> : EntityStateRepository<Event, TContext>, IEventRepository
    where TContext : DbContext, IEntityStateDbContext
{
    protected EventRepository(ILogger<Event> logger, TContext context) : base(logger, context)
    {
    }

    public override Task CreateRangeAsync(IReadOnlyCollection<Event> entities, CancellationToken? ctToken = null)
    {
        foreach (var item in entities)
        {
            item.StateId = (int) Shared.Infrastructure.Persistense.Enums.States.Ready; 
            item.StepId = (int)Core.Domain.Persistense.Entities.Enums.Steps.Calculating;
        }

        return base.CreateRangeAsync(entities, ctToken);
    }
}