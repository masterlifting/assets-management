﻿using IM.Service.Common.Net.RabbitServices;
using IM.Service.Company.Data.Services.MqServices;
using IM.Service.Company.Data.Settings;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Threading;
using System.Threading.Tasks;

namespace IM.Service.Company.Data.Services.BackgroundServices
{
    public class RabbitBackgroundService : BackgroundService
    {
        private readonly RabbitActionService service;
        private readonly RabbitSubscriber subscriber;

        public RabbitBackgroundService(ILogger<RabbitSubscriber> logger, IOptions<ServiceSettings> options, RabbitActionService service)
        {
            var targetExchanges = new[] { QueueExchanges.Sync, QueueExchanges.Function, QueueExchanges.Transfer };
            var targetQueues = new[] { QueueNames.CompanyData};
           
            this.service = service;
            subscriber = new RabbitSubscriber(logger, options.Value.ConnectionStrings.Mq, targetExchanges, targetQueues);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                subscriber.Unsubscribe();
                return Task.CompletedTask;
            }

            subscriber.Subscribe(service.GetActionResultAsync);

            return Task.CompletedTask;
        }
        public override Task StopAsync(CancellationToken stoppingToken)
        {
            base.StopAsync(stoppingToken);
            subscriber.Unsubscribe();
            return Task.CompletedTask;
        }
    }
}
