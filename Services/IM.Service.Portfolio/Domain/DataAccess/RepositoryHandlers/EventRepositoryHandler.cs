﻿using IM.Service.Common.Net.RepositoryService;
using IM.Service.Portfolio.Domain.Entities;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IM.Service.Portfolio.Domain.DataAccess.RepositoryHandlers;

public class EventRepositoryHandler : RepositoryHandler<Event>
{
    private readonly DatabaseContext context;
    public EventRepositoryHandler(DatabaseContext context) => this.context = context;

    public override async Task<IEnumerable<Event>> RunUpdateRangeHandlerAsync(IEnumerable<Event> entities)
    {
        entities = entities.ToArray();
        var existEntities = await GetExist(entities).ToArrayAsync();

        var result = existEntities
            .Join(entities, x => x.Id, y => y.Id, (x, y) => (Old: x, New: y))
            .ToArray();

        foreach (var (Old, New) in result)
        {
            Old.Cost = New.CurrencyId;
            Old.Info = New.Info;
            Old.DerivativeId = New.DerivativeId;
            Old.ExchangeId = New.ExchangeId;
            Old.AccountBrokerId = New.AccountBrokerId;
            Old.AccountUserId = New.AccountUserId;
            Old.AccountName = New.AccountName;
            Old.EventTypeId = New.EventTypeId;
            Old.CurrencyId = New.CurrencyId;
        }

        return result.Select(x => x.Old);
    }
    public override IQueryable<Event> GetExist(IEnumerable<Event> entities)
    {
        entities = entities.ToArray();

        var ids = entities
            .GroupBy(x => x.Id)
            .Select(x => x.Key);

        return context.Events.Where(x => ids.Contains(x.Id));
    }
}