using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Market.Services.RabbitMq.Function;
using AM.Services.Market.Settings;
using Microsoft.Extensions.Options;

namespace AM.Services.Market.Services.RabbitMq;

public class RabbitAction : RabbitActionBase
{
    public RabbitAction(IOptions<ServiceSettings> options, ILogger<RabbitAction> logger, IServiceScopeFactory scopeFactory) : base(options.Value.ConnectionStrings.Mq, logger, new()
    {
        { QueueExchanges.Function, new RabbitFunction(scopeFactory) },
    }) { }
}