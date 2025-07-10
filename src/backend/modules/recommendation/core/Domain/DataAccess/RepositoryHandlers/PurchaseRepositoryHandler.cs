﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AM.Services.Common.Contracts.SqlAccess;
using AM.Services.Recommendations.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AM.Services.Recommendations.Domain.DataAccess.RepositoryHandlers;

public class PurchaseRepositoryHandler : RepositoryHandler<Purchase>
{
    private readonly DatabaseContext context;
    public PurchaseRepositoryHandler(DatabaseContext context) => this.context = context;

    public override async Task<IEnumerable<Purchase>> RunUpdateRangeHandlerAsync(IReadOnlyCollection<Purchase> entities)
    {
        var existEntities = await GetExist(entities).ToArrayAsync();

        var result = existEntities
            .Join(entities, x => x.Id, y => y.Id, (x, y) => (Old: x, New: y))
            .ToArray();

        foreach (var (Old, New) in result)
        {
            Old.AssetId = New.AssetId;
            Old.AssetTypeId = New.AssetTypeId;
            
            Old.DiscountPlan = New.DiscountPlan;
            Old.DiscountFact = New.DiscountFact;
            Old.PricePlan = New.PricePlan;
            Old.PriceFact = New.PriceFact;
            Old.PriceNext = New.PriceNext;
        }

        return result.Select(x => x.Old);
    }
    public override IQueryable<Purchase> GetExist(IEnumerable<Purchase> entities)
    {
        entities = entities.ToArray();

        var ids = entities
            .GroupBy(x => x.Id)
            .Select(x => x.Key);

        return context.Purchases.Where(x => ids.Contains(x.Id));
    }
}