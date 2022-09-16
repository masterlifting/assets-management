using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Enums;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;
using AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Infrastructure.Persistense.Entities.EntityState;
using Shared.Infrastructure.Persistense.Enums;
using Shared.Infrastructure.Persistense.Repositories.Implementation;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

public sealed class ReportRepository<TContext> : EntityStateRepository<Report, TContext>, IReportRepository
    where TContext : DbContext, IEntityStateDbContext
{
    public ReportRepository(ILogger<Report> logger, TContext context) : base(logger, context) { }

    public override Task CreateAsync(Report entity, CancellationToken? ctToken = null)
    {
        entity.StateId = (int)States.Ready;
        entity.StepId = (int)Steps.Parsing;

        return base.CreateAsync(entity, ctToken);
    }
    public override Task CreateRangeAsync(IReadOnlyCollection<Report> entities, CancellationToken? ctToken = null)
    {
        foreach (var entity in entities)
        {
            entity.StateId = (int)States.Ready;
            entity.StepId = (int)Steps.Parsing;
        }

        return base.CreateRangeAsync(entities, ctToken);
    }

    public async Task<(DateOnly dateStart, DateOnly dateEnd)[]> GetReportDatesAsync(int accountId, ProviderId providerId, DateOnly dateStart, CancellationToken cToken) => await DbSet
        .Where(x =>
            x.AccountId == accountId
            && x.ProviderId == providerId.AsInt
            && x.DateStart.HasValue
            && x.DateEnd.HasValue
            && x.DateStart >= dateStart)
        .Select(x => ValueTuple.Create(x.DateStart!.Value, x.DateEnd!.Value))
        .ToArrayAsync(cToken);
}