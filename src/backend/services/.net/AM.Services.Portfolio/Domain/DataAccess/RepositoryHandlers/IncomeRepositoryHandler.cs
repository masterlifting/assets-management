using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AM.Services.Common.Contracts.SqlAccess;
using AM.Services.Portfolio.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AM.Services.Portfolio.Domain.DataAccess.RepositoryHandlers;

public class IncomeRepositoryHandler : RepositoryHandler<Income>
{
    private readonly DatabaseContext context;
    public IncomeRepositoryHandler(DatabaseContext context) => this.context = context;

    public override async Task<IEnumerable<Income>> RunUpdateRangeHandlerAsync(IReadOnlyCollection<Income> entities)
    {
        var existEntities = await GetExist(entities).ToArrayAsync();

        var result = existEntities
            .Join(entities, x => x.Id, y => y.Id, (x, y) => (Old: x, New: y))
            .ToArray();

        foreach (var (Old, New) in result)
        {
            Old.Value = New.Value;
            Old.Date = New.Date;
            Old.DealId = New.DealId;
        }

        return result.Select(x => x.Old);
    }
    public override IQueryable<Income> GetExist(IEnumerable<Income> entities)
    {
        entities = entities.ToArray();

        var ids = entities
            .GroupBy(x => x.Id)
            .Select(x => x.Key);

        return context.Incomes.Where(x => ids.Contains(x.Id));
    }
}