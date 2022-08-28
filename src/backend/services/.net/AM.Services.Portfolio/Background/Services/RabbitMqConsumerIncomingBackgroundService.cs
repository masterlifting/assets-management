using AM.Services.Portfolio.Background.ServiceTasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Shared.Core.Abstractions.Background.MessageQueue;
using Shared.Core.Abstractions.Queues;
using Shared.Core.Background.MessageQueue;
using Shared.Core.Domains.RabbitMq;

namespace AM.Services.Portfolio.Background.Services;

public class RabbitMqConsumerIncomingBackgroundService : MqConsumerBackgroundService<RabbitMqConsumerSettings>
{
    protected RabbitMqConsumerIncomingBackgroundService(
        IServiceScopeFactory scopeFactory
        , IOptionsMonitor<IMqConsumerSection<RabbitMqConsumerSettings>> options
        , ILogger<RabbitMqConsumerIncomingBackgroundService> logger
        , IMqConsumer consumer) : base(options, logger, consumer, new RabbitMqConsumerIncomingBackgroundTask(scopeFactory))
    {
    }
}