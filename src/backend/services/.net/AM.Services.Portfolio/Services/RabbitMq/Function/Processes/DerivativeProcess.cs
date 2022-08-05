using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AM.Services.Common.Contracts.Helpers;
using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Portfolio.Domain.Entities;
using AM.Services.Portfolio.Services.Entity;

namespace AM.Services.Portfolio.Services.RabbitMq.Function.Processes;

public sealed class DerivativeProcess : IRabbitProcess
{
    private readonly DerivativeService service;
    private readonly RabbitAction rabbitAction;

    public DerivativeProcess(DerivativeService service, RabbitAction rabbitAction)
    {
        this.service = service;
        this.rabbitAction = rabbitAction;
    }

    public Task ProcessAsync<T>(QueueActions action, T model) where T : class => action switch
    {
        QueueActions.Compute => model switch
        {
            Derivative derivative => service.GetTransferModelAsync(derivative)
                .ContinueWith(async x => 
                    rabbitAction
                    .Publish(QueueExchanges.Transfer, QueueNames.Recommendations, QueueEntities.Deal, QueueActions.Set, await x)),
            _ => service.Logger.LogDefaultTask($"{action} {typeof(T).Name}"),
        },
        _ => service.Logger.LogDefaultTask($"{action}")
    };
    public Task ProcessRangeAsync<T>(QueueActions action, IEnumerable<T> models) where T : class => action switch
    {
        QueueActions.Compute => models switch
        {
            Derivative[] derivatives => service.GetTransferModelsAsync(derivatives)
                .ContinueWith(async x =>
                {
                    var result = await x;
                    if (result.Any())
                        rabbitAction.Publish(QueueExchanges.Transfer, QueueNames.Recommendations, QueueEntities.Deals, QueueActions.Set, result);
                }),
            _ => service.Logger.LogDefaultTask($"{action} {typeof(T).Name}s"),
        },
        _ => service.Logger.LogDefaultTask($"{action}")
    };
}