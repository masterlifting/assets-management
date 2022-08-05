using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AM.Services.Common.Contracts.SqlAccess;
using AM.Services.Portfolio.Domain.Entities.Catalogs;
using Microsoft.EntityFrameworkCore;

namespace AM.Services.Portfolio.Domain.DataAccess.RepositoryHandlers;

public class EventTypeRepositoryHandler : RepositoryHandler<EventType>
{
    private readonly DatabaseContext context;
    public EventTypeRepositoryHandler(DatabaseContext context) => this.context = context;

    public override async Task<IEnumerable<EventType>> RunUpdateRangeHandlerAsync(IReadOnlyCollection<EventType> entities)
    {
        var existEntities = await GetExist(entities).ToArrayAsync();

        var result = existEntities
            .Join(entities, x => x.Id, y => y.Id, (x, y) => (Old: x, New: y))
            .ToArray();

        foreach (var (Old, New) in result)
        {
            Old.Name = New.Name;
            Old.OperationTypeId = New.OperationTypeId;
            Old.Description = New.Description;
        }

        return result.Select(x => x.Old);
    }
    public override IQueryable<EventType> GetExist(IEnumerable<EventType> entities)
    {
        entities = entities.ToArray();

        var ids = entities
            .GroupBy(x => x.Id)
            .Select(x => x.Key);

        return context.EventTypes.Where(x => ids.Contains(x.Id));
    }
}