﻿using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Common.Contracts.SqlAccess;
using AM.Services.Market.Domain.Entities;
using AM.Services.Market.Services.RabbitMq;
using Microsoft.EntityFrameworkCore;
using static AM.Services.Common.Contracts.Enums;


namespace AM.Services.Market.Domain.DataAccess.RepositoryHandlers;

public class CoefficientRepositoryHandler : RepositoryHandler<Coefficient>
{
    private readonly DatabaseContext context;
    private readonly RabbitAction rabbitAction;
    public CoefficientRepositoryHandler(RabbitAction rabbitAction, DatabaseContext context)
    {
        this.context = context;
        this.rabbitAction = rabbitAction;
    }

    public override async Task<IEnumerable<Coefficient>> RunUpdateRangeHandlerAsync(IReadOnlyCollection<Coefficient> entities)
    {
        var existEntities = await GetExist(entities).ToArrayAsync();

        var result = existEntities
            .Join(entities,
                x => (x.CompanyId, x.SourceId, x.Year, x.Quarter),
                y => (y.CompanyId, y.SourceId, y.Year, y.Quarter),
                (x, y) => (Old: x, New: y))
            .ToArray();

        foreach (var (Old, New) in result)
        {
            Old.Pe = New.Pe;
            Old.Pb = New.Pb;
            Old.DebtLoad = New.DebtLoad;
            Old.Profitability = New.Profitability;
            Old.Roa = New.Roa;
            Old.Roe = New.Roe;
            Old.Eps = New.Eps;

            Old.StatusId = New.StatusId;
            Old.Result = New.Result;
        }

        return result.Select(x => x.Old);
    }
    public override IQueryable<Coefficient> GetExist(IEnumerable<Coefficient> entities)
    {
        entities = entities.ToArray();

        var companyIds = entities
            .GroupBy(x => x.CompanyId)
            .Select(x => x.Key);
        var sourceIds = entities
            .GroupBy(x => x.SourceId)
            .Select(x => x.Key);
        var years = entities
            .GroupBy(x => x.Year)
            .Select(x => x.Key);
        var quarters = entities
            .GroupBy(x => x.Quarter)
            .Select(x => x.Key);

        return context.Coefficients
            .Where(x =>
                companyIds.Contains(x.CompanyId)
                && sourceIds.Contains(x.SourceId)
                && years.Contains(x.Year)
                && quarters.Contains(x.Quarter));
    }

    public override async Task RunPostProcessAsync(RepositoryActions action, Coefficient entity)
    {
        if (action is not RepositoryActions.Delete)
            return;

        var lastEntity = await context.Coefficients.Where(x =>
            x.CompanyId == entity.CompanyId
            && x.SourceId == entity.SourceId
            && x.Year < entity.Year || x.Year == entity.Year && x.Quarter < entity.Quarter)
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Quarter)
            .LastOrDefaultAsync();

        if (lastEntity is not null)
            rabbitAction.Publish(QueueExchanges.Function, QueueNames.Market, QueueEntities.Coefficient, QueueActions.Set, lastEntity);
        else
            rabbitAction.Publish(QueueExchanges.Function, QueueNames.Market, QueueEntities.Rating, QueueActions.Compute, new Rating());
    }
    public override async Task RunPostProcessRangeAsync(RepositoryActions action, IReadOnlyCollection<Coefficient> entities)
    {
        if (action is not RepositoryActions.Delete)
            return;

        var lastEntities = new List<Coefficient>(entities.Count);
        foreach (var group in entities.GroupBy(x => (x.CompanyId, x.SourceId)))
        {
            var minEntity = group.MinBy(x => (x.Year, x.Quarter))!;

            var lastEntity = await context.Coefficients.Where(x =>
                x.CompanyId == group.Key.CompanyId
                && x.SourceId == group.Key.SourceId
                && x.Year < minEntity.Year || x.Year == minEntity.Year && x.Quarter < minEntity.Quarter)
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Quarter)
                .LastOrDefaultAsync();

            if (lastEntity is not null)
                lastEntities.Add(lastEntity);
        }

        if (lastEntities.Any())
            rabbitAction.Publish(QueueExchanges.Function, QueueNames.Market, QueueEntities.Coefficients, QueueActions.Set, lastEntities);
        else
            rabbitAction.Publish(QueueExchanges.Function, QueueNames.Market, QueueEntities.Ratings, QueueActions.Compute, Array.Empty<Rating>());
    }
}