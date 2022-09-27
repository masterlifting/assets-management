using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Context;
using Shared.Persistense.Repositories;

using static Shared.Persistense.Constants.Enums;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories
{
    public sealed class EventRepository<TContext> : EntityStateRepository<Event, TContext>, IEventRepository
        where TContext : DbContext, IEntityStateDbContext
    {
        public EventRepository(ILogger<Event> logger, TContext context) : base(logger, context)
        {
        }

        public override Task CreateRangeAsync(IReadOnlyCollection<Event> entities, CancellationToken? cToken = null)
        {
            foreach (var item in entities)
            {
                item.StateId = (int)States.Ready;
                item.StepId = (int)Steps.Computing;
            }

            return base.CreateRangeAsync(entities, cToken);
        }
    }
}