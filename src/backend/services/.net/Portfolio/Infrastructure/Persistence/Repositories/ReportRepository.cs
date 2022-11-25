using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Context;
using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public sealed class ReportRepository<TContext> : EntityRepository<Report, TContext>, IReportRepository
    where TContext : DbContext, IEntityStateDbContext
{
    private readonly TContext _context;
    public ReportRepository(ILogger<Report> logger, TContext context) : base(logger, context)
    {
        _context = context;
    }

    public async Task<(DateOnly dateStart, DateOnly dateEnd)[]> GetReportsDatesAsync(int accountId, DateOnly dateStart, CancellationToken cToken) =>
        await _context.Set<Report>()
        .Where(x => x.AccountId == accountId && x.DateStart >= dateStart)
        .Select(x => ValueTuple.Create(x.DateStart, x.DateEnd))
        .ToArrayAsync(cToken);
}