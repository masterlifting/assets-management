using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Recommendations.Services.RabbitMq.Sync;
using AM.Services.Recommendations.Services.RabbitMq.Transfer;
using AM.Services.Recommendations.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AM.Services.Recommendations.Services.RabbitMq;

public class RabbitAction : RabbitActionBase
{
    public RabbitAction(IOptions<ServiceSettings> options, ILogger<RabbitAction> logger, IServiceScopeFactory scopeFactory) : base(options.Value.ConnectionStrings.Mq, logger, new()

    {
        { QueueExchanges.Sync, new RabbitSync(scopeFactory) },
        { QueueExchanges.Transfer, new RabbitTransfer(scopeFactory) },
    })
    { }
}