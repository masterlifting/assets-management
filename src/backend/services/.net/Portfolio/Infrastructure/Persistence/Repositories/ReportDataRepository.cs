using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Context;
using Shared.Persistense.Repositories;

using static Shared.Persistense.Constants.Enums;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public sealed class ReportDataRepository<TContext> : EntityStateRepository<ReportData, TContext>, IReportDataRepository
    where TContext : DbContext, IEntityStateDbContext
{
    public ReportDataRepository(ILogger<ReportData> logger, TContext context) : base(logger, context) { }

    public override Task CreateAsync(ReportData entity, CancellationToken? ctToken = null)
    {
        entity.StepId = (int)Steps.Parsing;
        return base.CreateAsync(entity, ctToken);
    }
    public override Task CreateRangeAsync(IReadOnlyCollection<ReportData> entities, CancellationToken? cToken = null)
    {
        foreach (var entity in entities)
            entity.StepId = (int)Steps.Parsing;

        return base.CreateRangeAsync(entities, cToken);
    }
}