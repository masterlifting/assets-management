using AM.Services.Common.Contracts.SqlAccess;
using AM.Services.Portfolio.Domain.Entities;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AM.Services.Portfolio.Domain.DataAccess.RepositoryHandlers;

public class ReportRepositoryHandler : RepositoryHandler<Report>
{
    private readonly DatabaseContext _context;
    public ReportRepositoryHandler(DatabaseContext context) => _context = context;

    public override async Task<IEnumerable<Report>> RunUpdateRangeHandlerAsync(IReadOnlyCollection<Report> entities)
    {
        var existEntities = await GetExist(entities).ToArrayAsync();

        var result = existEntities
            .Join(entities,
                x => (x.ReportFileId, x.AccountId),
                y => (y.ReportFileId, y.AccountId),
                (x, y) => (Old: x, New: y))
            .ToArray();

        foreach (var (oldEntity, newEntity) in result)
        {
            oldEntity.DateStart = newEntity.DateStart;
            oldEntity.DateEnd = newEntity.DateEnd;
        }

        return result.Select(x => x.Old);
    }
    public override IQueryable<Report> GetExist(IEnumerable<Report> entities)
    {
        entities = entities.ToArray();

        var reportFileIds = entities
            .GroupBy(x => x.ReportFileId)
            .Select(x => x.Key);
        var accountIds = entities
            .GroupBy(x => x.AccountId)
            .Select(x => x.Key);

        return _context.Reports.Where(x => reportFileIds.Contains(x.ReportFileId)&& accountIds.Contains(x.AccountId));
    }
}