using System.Threading.Tasks;
using AM.Services.Common.Contracts.Helpers;
using AM.Services.Common.Contracts.Models.RabbitMq.Api;
using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Recommendations.Services.Entity;
using AM.Services.Recommendations.Services.RabbitMq.Transfer.Processes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AM.Services.Recommendations.Services.RabbitMq.Transfer;

public class RabbitTransfer : IRabbitAction
{
    private readonly IServiceScopeFactory scopeFactory;
    public RabbitTransfer(IServiceScopeFactory scopeFactory) => this.scopeFactory = scopeFactory;

    public Task GetResultAsync(QueueEntities entity, QueueActions action, string data)
    {
        var serviceProvider = scopeFactory.CreateAsyncScope().ServiceProvider;
        var logger = serviceProvider.GetRequiredService<ILogger<RabbitTransfer>>();

        return entity switch
        {
            QueueEntities.Ratings => Task.Run(async () =>
            {
                var dtos = JsonHelper.Deserialize<AssetRatingMqDto[]>(data);
                var assets = await serviceProvider.GetRequiredService<AssetService>().SetAsync(dtos);

                await serviceProvider.GetRequiredService<SaleProcess>().ProcessRangeAsync(action, assets);
                await serviceProvider.GetRequiredService<PurchaseProcess>().ProcessRangeAsync(action, assets);
            }),
            QueueEntities.Deal => Task.Run(() =>
                serviceProvider.GetRequiredService<SaleProcess>().ProcessAsync(action, JsonHelper.Deserialize<AssetPortfolioMqDto>(data))),
            QueueEntities.Deals => Task.Run(() => 
                serviceProvider.GetRequiredService<SaleProcess>().ProcessRangeAsync(action, JsonHelper.Deserialize<AssetPortfolioMqDto[]>(data))),
            QueueEntities.Price => Task.Run(() => 
                serviceProvider.GetRequiredService<AssetProcess>().ProcessAsync(action, JsonHelper.Deserialize<AssetMarketMqDto>(data))),
            QueueEntities.Prices => Task.Run(() => 
                serviceProvider.GetRequiredService<AssetProcess>().ProcessRangeAsync(action, JsonHelper.Deserialize<AssetMarketMqDto[]>(data))),
            _ => logger.LogDefaultTask(entity.ToString())
        };
    }
}