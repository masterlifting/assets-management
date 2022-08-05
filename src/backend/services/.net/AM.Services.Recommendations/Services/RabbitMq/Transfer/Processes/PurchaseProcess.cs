using System.Collections.Generic;
using System.Threading.Tasks;
using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Recommendations.Domain.Entities;
using AM.Services.Recommendations.Services.Entity;

namespace AM.Services.Recommendations.Services.RabbitMq.Transfer.Processes;

public class PurchaseProcess : IRabbitProcess
{
    private readonly PurchaseService purchaseService;
    public PurchaseProcess(PurchaseService purchaseService) => this.purchaseService = purchaseService;

    public Task ProcessAsync<T>(QueueActions action, T model) where T : class => Task.CompletedTask;
    public Task ProcessRangeAsync<T>(QueueActions action, IEnumerable<T> models) where T : class => action switch
    {
        QueueActions.Create or QueueActions.Update => models switch
        {
            Asset[] assets => purchaseService.SetAsync(assets),
            _ => Task.CompletedTask
        },
        QueueActions.Delete => models switch
        {
            Asset[] assets => purchaseService.DeleteAsync(assets),
            _ => Task.CompletedTask
        },
        _ => Task.CompletedTask
    };
}