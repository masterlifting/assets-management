using System.Collections.Generic;
using System.Threading.Tasks;
using AM.Services.Common.Contracts.Models.RabbitMq.Api;
using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Recommendations.Services.Entity;

namespace AM.Services.Recommendations.Services.RabbitMq.Transfer.Processes;

public class AssetProcess : IRabbitProcess
{
    private readonly AssetService service;
    public AssetProcess(AssetService service) => this.service = service;

    public Task ProcessAsync<T>(QueueActions action, T model) where T : class => action switch
    {
        QueueActions.Create or QueueActions.Update or QueueActions.Delete => model switch
        {
            AssetMarketMqDto dto => service.SetAsync(dto),
            _ => Task.CompletedTask
        },
        _ => Task.CompletedTask
    };
    public Task ProcessRangeAsync<T>(QueueActions action, IEnumerable<T> models) where T : class => action switch
    {
        QueueActions.Create or QueueActions.Update or QueueActions.Delete => models switch
        {
            AssetMarketMqDto[] dtos => service.SetAsync(dtos),
            _ => Task.CompletedTask
        },
        _ => Task.CompletedTask
    };
}