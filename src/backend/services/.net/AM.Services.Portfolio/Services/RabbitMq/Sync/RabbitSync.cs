using System.Threading.Tasks;
using AM.Services.Common.Contracts.Helpers;
using AM.Services.Common.Contracts.Models.RabbitMq.Api;
using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Portfolio.Services.RabbitMq.Sync.Processes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AM.Services.Portfolio.Services.RabbitMq.Sync;

public class RabbitSync : IRabbitAction
{
    private readonly IServiceScopeFactory scopeFactory;
    public RabbitSync(IServiceScopeFactory scopeFactory) => this.scopeFactory = scopeFactory;

    public Task GetResultAsync(QueueEntities entity, QueueActions action, string data)
    {
        var serviceProvider = scopeFactory.CreateAsyncScope().ServiceProvider;

        return entity switch
        {
            QueueEntities.Asset => serviceProvider.GetRequiredService<AssetProcess>().ProcessAsync(action, JsonHelper.Deserialize<AssetMqDto>(data)),
            QueueEntities.Assets => serviceProvider.GetRequiredService<AssetProcess>().ProcessRangeAsync(action, JsonHelper.Deserialize<AssetMqDto[]>(data)),
            _ => serviceProvider.GetRequiredService<ILogger<RabbitSync>>().LogDefaultTask($"{action} {entity}")
        };
    }
}