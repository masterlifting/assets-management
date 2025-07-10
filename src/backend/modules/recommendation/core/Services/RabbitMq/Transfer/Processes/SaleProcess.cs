using System.Collections.Generic;
using System.Threading.Tasks;
using AM.Services.Common.Contracts.Models.RabbitMq.Api;
using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Recommendations.Domain.Entities;
using AM.Services.Recommendations.Services.Entity;

namespace AM.Services.Recommendations.Services.RabbitMq.Transfer.Processes;

public class SaleProcess : IRabbitProcess
{
    private readonly SaleService saleService;
    private readonly AssetService assetService;

    public SaleProcess(SaleService saleService, AssetService assetService)
    {
        this.saleService = saleService;
        this.assetService = assetService;
    }

    public Task ProcessAsync<T>(QueueActions action, T model) where T : class => action switch
    {
        QueueActions.Set => model switch
        {
            AssetPortfolioMqDto data => assetService.SetAsync(data).ContinueWith(async x => saleService.SetAsync(await x)),
            _ => Task.CompletedTask
        },
        _ => Task.CompletedTask
    };
    public Task ProcessRangeAsync<T>(QueueActions action, IEnumerable<T> models) where T : class => action switch
    {
        QueueActions.Set => models switch
        {
            AssetPortfolioMqDto[] data => assetService.SetAsync(data).ContinueWith(x => saleService.SetAsync(x)),
            _ => Task.CompletedTask
        },
        QueueActions.Create or QueueActions.Update => models switch
        {
            Asset[] data => saleService.SetAsync(data),
            _ => Task.CompletedTask
        },
        QueueActions.Delete => models switch
        {
            Asset[] data => saleService.DeleteAsync(data),
            _ => Task.CompletedTask
        },
        _ => Task.CompletedTask
    };
}