using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Market.Services.RabbitMq;
using AM.Services.Market.Settings;
using Microsoft.Extensions.Options;

namespace AM.Services.Market.Services.Background;

public class RabbitBackgroundService : BackgroundService
{
    private readonly RabbitAction action;
    private readonly RabbitSubscriber subscriber;

    public RabbitBackgroundService(RabbitAction action, ILogger<RabbitBackgroundService> logger, IOptions<ServiceSettings> options)
    {
        this.action = action;
        subscriber = new RabbitSubscriber(
            logger,
            options.Value.ConnectionStrings.Mq,
            new[] { QueueExchanges.Sync, QueueExchanges.Function, QueueExchanges.Transfer },
            QueueNames.Market);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
        {
            subscriber.Unsubscribe();
            return Task.CompletedTask;
        }

        subscriber.Subscribe(action.StartAsync);

        return Task.CompletedTask;
    }
    public override Task StopAsync(CancellationToken stoppingToken)
    {
        subscriber.Unsubscribe();
        base.StopAsync(stoppingToken);
        return Task.CompletedTask;
    }
}