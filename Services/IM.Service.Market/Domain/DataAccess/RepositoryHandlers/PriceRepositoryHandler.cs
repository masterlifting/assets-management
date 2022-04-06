﻿using IM.Service.Common.Net.RabbitServices;
using IM.Service.Common.Net.RabbitServices.Configuration;
using IM.Service.Common.Net.RepositoryService;
using IM.Service.Market.Domain.DataAccess.Comparators;
using IM.Service.Market.Domain.Entities;
using IM.Service.Market.Settings;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using static IM.Service.Common.Net.Enums;
using static IM.Service.Market.Enums;

namespace IM.Service.Market.Domain.DataAccess.RepositoryHandlers;

public class PriceRepositoryHandler : RepositoryHandler<Price, DatabaseContext>
{
    private readonly DatabaseContext context;
    private readonly string rabbitConnectionString;

    public PriceRepositoryHandler(IOptions<ServiceSettings> options, DatabaseContext context) : base(context)
    {
        this.context = context;
        rabbitConnectionString = options.Value.ConnectionStrings.Mq;
    }

    public override async Task<IEnumerable<Price>> RunUpdateRangeHandlerAsync(IEnumerable<Price> entities)
    {
        entities = entities.ToArray();
        var existEntities = await GetExist(entities).ToArrayAsync();

        var result = existEntities
            .Join(entities,
                x => (x.CompanyId, x.SourceId, x.Date),
                y => (y.CompanyId, y.SourceId, y.Date),
                (x, y) => (Old: x, New: y))
            .ToArray();

        foreach (var (Old, New) in result)
        {
            Old.CurrencyId = New.CurrencyId;

            Old.Value = New.Value;
            Old.ValueTrue = New.ValueTrue;

            Old.StatusId = New.StatusId;
            Old.Result = New.Result;
        }

        return result.Select(x => x.Old);
    }
    public override async Task<IEnumerable<Price>> RunDeleteRangeHandlerAsync(IEnumerable<Price> entities)
    {
        var comparer = new DataDateComparer<Price>();
        var result = new List<Price>();

        foreach (var group in entities.GroupBy(x => x.CompanyId))
        {
            var dbEntities = await context.Prices.Where(x => x.CompanyId.Equals(group.Key)).ToArrayAsync();
            result.AddRange(dbEntities.Except(group, comparer));
        }

        return result;
    }

    public override Task RunPostProcessAsync(RepositoryActions action, Price entity)
    {
        if (action is RepositoryActions.Update && entity.StatusId is not (byte)Statuses.New)
            return Task.CompletedTask;

        var publisher = new RabbitPublisher(rabbitConnectionString, QueueExchanges.Function);
        publisher.PublishTask(QueueNames.MarketData, QueueEntities.Price, RabbitHelper.GetQueueAction(action), entity);

        return Task.CompletedTask;
    }
    public override Task RunPostProcessAsync(RepositoryActions action, IReadOnlyCollection<Price> entities)
    {
        if (action is RepositoryActions.Update && !entities.Any(x => x.StatusId is (byte)Statuses.New))
            return Task.CompletedTask;

        var publisher = new RabbitPublisher(rabbitConnectionString, QueueExchanges.Function);

        publisher.PublishTask(QueueNames.MarketData, QueueEntities.Prices, RabbitHelper.GetQueueAction(action),
            action is RepositoryActions.Update
                ? entities.Where(x => x.StatusId is (byte) Statuses.New)
                : entities);

        return Task.CompletedTask;
    }

    public override IQueryable<Price> GetExist(IEnumerable<Price> entities)
    {
        entities = entities.ToArray();

        var companyIds = entities
            .GroupBy(x => x.CompanyId)
            .Select(x => x.Key);
        var sourceIds = entities
            .GroupBy(x => x.SourceId)
            .Select(x => x.Key);
        var dates = entities
            .GroupBy(x => x.Date)
            .Select(x => x.Key);

        return context.Prices
            .Where(x =>
                companyIds.Contains(x.CompanyId)
                && sourceIds.Contains(x.SourceId)
                && dates.Contains(x.Date));
    }
}