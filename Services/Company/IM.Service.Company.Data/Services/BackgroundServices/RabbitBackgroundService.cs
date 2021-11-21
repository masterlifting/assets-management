﻿using IM.Service.Common.Net.RabbitServices;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using System;
using System.Threading;
using System.Threading.Tasks;
using IM.Service.Company.Data.Services.MqServices;
using IM.Service.Company.Data.Settings;
using Microsoft.Extensions.Logging;

namespace IM.Service.Company.Data.Services.BackgroundServices
{
    public class RabbitBackgroundService : BackgroundService
    {
        private readonly RabbitSubscriber subscriber;
        private readonly IServiceScope scope;

        public RabbitBackgroundService(ILogger<RabbitSubscriber> logger, IServiceProvider services, IOptions<ServiceSettings> options)
        {
            var targetExchanges = new[] { QueueExchanges.Sync, QueueExchanges.Function, QueueExchanges.Transfer };
            var targetQueues = new[] { QueueNames.CompanyData};
            subscriber = new RabbitSubscriber(logger, options.Value.ConnectionStrings.Mq, targetExchanges, targetQueues);
            scope = services.CreateScope();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                subscriber.Unsubscribe();
                scope.Dispose();
                return Task.CompletedTask;
            }

            var rabbitService = scope.ServiceProvider.GetRequiredService<RabbitActionService>();
            subscriber.Subscribe(rabbitService.GetActionResultAsync);

            return Task.CompletedTask;
        }
        public override Task StopAsync(CancellationToken stoppingToken)
        {
            base.StopAsync(stoppingToken);
            subscriber.Unsubscribe();
            scope.Dispose();
            return Task.CompletedTask;
        }
    }
}