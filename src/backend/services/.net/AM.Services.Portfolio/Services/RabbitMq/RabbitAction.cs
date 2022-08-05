using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Portfolio.Services.RabbitMq.Function;
using AM.Services.Portfolio.Services.RabbitMq.Sync;
using AM.Services.Portfolio.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AM.Services.Portfolio.Services.RabbitMq;

public class RabbitAction : RabbitActionBase
{
    public RabbitAction(IOptions<ServiceSettings> options, ILogger<RabbitAction> logger, IServiceScopeFactory scopeFactory) : base(options.Value.ConnectionStrings.Mq,  logger, new()
    {
        { QueueExchanges.Function, new RabbitFunction(scopeFactory) },
        { QueueExchanges.Sync, new RabbitSync(scopeFactory) }
    }) { }
}