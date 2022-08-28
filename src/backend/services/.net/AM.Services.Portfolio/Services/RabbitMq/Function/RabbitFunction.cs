using System.Threading.Tasks;
using AM.Services.Common.Contracts.Helpers;
using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Portfolio.Domain.Entities;
using AM.Services.Portfolio.Models.Api.Mq;
using AM.Services.Portfolio.Services.RabbitMq.Function.Processes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AM.Services.Portfolio.Services.RabbitMq.Function;

public class RabbitFunction : IRabbitAction
{
    private readonly IServiceScopeFactory scopeFactory;
    public RabbitFunction(IServiceScopeFactory scopeFactory) => this.scopeFactory = scopeFactory;

    public Task GetResultAsync(QueueEntities entity, QueueActions action, string data)
    {
        var serviceProvider = scopeFactory.CreateAsyncScope().ServiceProvider;

        return entity switch
        {
            QueueEntities.Deal => serviceProvider.GetRequiredService<DealProcess>().ProcessAsync(action, JsonHelper.Deserialize<Deal>(data)),
            QueueEntities.Deals => serviceProvider.GetRequiredService<DealProcess>().ProcessRangeAsync(action, JsonHelper.Deserialize<Deal[]>(data)),
            QueueEntities.Event => serviceProvider.GetRequiredService<EventProcess>().ProcessAsync(action, JsonHelper.Deserialize<Event>(data)),
            QueueEntities.Events => serviceProvider.GetRequiredService<EventProcess>().ProcessRangeAsync(action, JsonHelper.Deserialize<Event[]>(data)),
            QueueEntities.Derivative => serviceProvider.GetRequiredService<DerivativeProcess>().ProcessAsync(action, JsonHelper.Deserialize<Derivative>(data)),
            QueueEntities.Derivatives => serviceProvider.GetRequiredService<DerivativeProcess>().ProcessRangeAsync(action, JsonHelper.Deserialize<Derivative[]>(data)),
            _ => serviceProvider.GetRequiredService<ILogger<RabbitFunction>>().LogDefaultTask($"{action} {entity}")
        };
    }
}