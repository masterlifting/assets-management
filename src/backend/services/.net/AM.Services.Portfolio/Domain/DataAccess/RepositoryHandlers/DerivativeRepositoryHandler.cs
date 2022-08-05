using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Common.Contracts.SqlAccess;
using AM.Services.Portfolio.Domain.Entities;
using AM.Services.Portfolio.Services.RabbitMq;
using Microsoft.EntityFrameworkCore;
using static AM.Services.Common.Contracts.Enums;

namespace AM.Services.Portfolio.Domain.DataAccess.RepositoryHandlers;

public class DerivativeRepositoryHandler : RepositoryHandler<Derivative>
{
    private readonly RabbitAction rabbitAction;
    private readonly DatabaseContext context;

    public DerivativeRepositoryHandler(RabbitAction rabbitAction, DatabaseContext context)
    {
        this.rabbitAction = rabbitAction;
        this.context = context;
    }

    public override async Task<IEnumerable<Derivative>> RunUpdateRangeHandlerAsync(IReadOnlyCollection<Derivative> entities)
    {
        var existEntities = await GetExist(entities).ToArrayAsync();

        var result = existEntities
            .Join(entities, x => (x.Id, x.Code), y => (y.Id, y.Code), (x, y) => (Old: x, New: y))
            .ToArray();

        foreach (var (Old, New) in result)
        {
            Old.UpdateTime = DateTime.UtcNow;
            Old.Balance += New.Balance;
            Old.AssetId = New.AssetId;
            Old.AssetTypeId = New.AssetTypeId;
        }

        return result.Select(x => x.Old);
    }
    public override IQueryable<Derivative> GetExist(IEnumerable<Derivative> entities)
    {
        entities = entities.ToArray();

        var ids = entities.Select(x => x.Id).Distinct();
        var codes = entities.Select(x => x.Code).Distinct();

        return context.Derivatives.Where(x => ids.Contains(x.Id) && codes.Contains(x.Code));
    }
    public override Task RunPostProcessAsync(RepositoryActions action, Derivative entity)
    {
        rabbitAction.Publish(QueueExchanges.Function, QueueNames.Portfolio, QueueEntities.Derivative, QueueActions.Compute, entity);
        return Task.CompletedTask;
    }
    public override Task RunPostProcessRangeAsync(RepositoryActions action, IReadOnlyCollection<Derivative> entities)
    {
        rabbitAction.Publish(QueueExchanges.Function, QueueNames.Portfolio, QueueEntities.Derivatives, QueueActions.Compute, entities);
        return Task.CompletedTask;
    }
}