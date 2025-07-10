﻿using AM.Services.Common.Contracts.Models.RabbitMq.Api;
using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Common.Contracts.SqlAccess;
using AM.Services.Market.Domain.Entities;
using AM.Services.Market.Services.RabbitMq;
using Microsoft.EntityFrameworkCore;
using static AM.Services.Common.Contracts.Enums;

namespace AM.Services.Market.Domain.DataAccess.RepositoryHandlers;

public class RatingRepositoryHandler : RepositoryHandler<Rating>
{
    private readonly DatabaseContext context;
    private readonly RabbitAction rabbitAction;
    public RatingRepositoryHandler(RabbitAction rabbitAction, DatabaseContext context)
    {
        this.context = context;
        this.rabbitAction = rabbitAction;
    }

    public override async Task<IEnumerable<Rating>> RunUpdateRangeHandlerAsync(IReadOnlyCollection<Rating> entities)
    {
        var existEntities = await GetExist(entities).ToArrayAsync();

        var result = existEntities
            .Join(entities, x => x.CompanyId, y => y.CompanyId, (x, y) => (Old: x, New: y))
            .ToArray();

        foreach (var (Old, New) in result)
        {
            Old.ResultCoefficient = New.ResultCoefficient;
            Old.ResultDividend = New.ResultDividend;
            Old.ResultPrice = New.ResultPrice;
            Old.ResultReport = New.ResultReport;

            Old.Result = New.Result;
        }

        return result.Select(x => x.Old);
    }
    public override IQueryable<Rating> GetExist(IEnumerable<Rating> entities)
    {
        entities = entities.ToArray();

        var companyIds = entities
            .GroupBy(x => x.CompanyId)
            .Select(x => x.Key);

        return context.Rating.Where(x => companyIds.Contains(x.CompanyId));
    }

    public override Task RunPostProcessRangeAsync(RepositoryActions action, IReadOnlyCollection<Rating> entities)
    {
        var ratings = entities.OrderByDescending(x => x.Result).Select((x, i) => new AssetRatingMqDto(x.CompanyId, (byte)AssetTypes.Stock, i + 1));

        rabbitAction.Publish(QueueExchanges.Transfer, QueueNames.Recommendations, QueueEntities.Ratings, RabbitHelper.GetAction(action), ratings);

        return Task.CompletedTask;
    }
}