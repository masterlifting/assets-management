﻿using IM.Service.Common.Net.Models.Configuration;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IM.Service.Common.Net.RabbitMQ.Configuration;
using static IM.Service.Common.Net.Helpers.LogHelper;
using static IM.Service.Common.Net.Helpers.ServiceHelper;

namespace IM.Service.Common.Net.RabbitMQ;

public class RabbitSubscriber
{
    private readonly ILogger<RabbitSubscriber> logger;
    private readonly SemaphoreSlim semaphore = new(1, 1);

    private readonly IModel channel;
    private readonly IConnection connection;
    private readonly List<Queue> queues;
    private readonly string[] queuesWithConfirm;

    public RabbitSubscriber(ILogger<RabbitSubscriber> logger, string connectionString, IEnumerable<QueueExchanges> exchangeNames, IReadOnlyCollection<QueueNames> queueNames)
    {
        this.logger = logger;
        var mqConnection = new SettingsConverter<ConnectionModel>(connectionString).Model;

        var factory = new ConnectionFactory
        {
            HostName = mqConnection.Server,
            UserName = mqConnection.UserId,
            Password = mqConnection.Password
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        queues = new List<Queue>(queueNames.Count);

        foreach (var exchange in QueueConfiguration.Exchanges.Join(exchangeNames, x => x.NameEnum, y => y, (x, _) => x))
        {
            channel.ExchangeDeclare(exchange.NameString, exchange.Type);

            foreach (var queue in exchange.Queues.Join(queueNames, x => x.NameEnum, y => y, (x, _) => x))
            {
                if (queues.Contains(queue, new QueueComparer()))
                    continue;

                channel.QueueDeclare(queue.NameString, false, false, false, null);
                queues.Add(queue);

                foreach (var route in queue.Entities)
                    channel.QueueBind(queue.NameString, exchange.NameString, $"{queue.NameString}.{route.NameString}.*");
            }
        }

        queuesWithConfirm = queues.Where(x => x.WithConfirm).Select(x => x.NameString).ToArray();
    }

    public void Subscribe(Func<QueueExchanges, string, string, Task<bool>> getActionResult)
    {
        foreach (var queue in queues)
            SubscribeQueue(getActionResult, queue);
    }
    public void Unsubscribe()
    {
        channel.Dispose();
        connection.Dispose();
    }
    private void SubscribeQueue(Func<QueueExchanges, string, string, Task<bool>> getActionResult, Queue queue)
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += OnReceivedAsync;
        channel.BasicConsume(queue.NameString, !queue.WithConfirm, consumer);

        async void OnReceivedAsync(object? _, BasicDeliverEventArgs ea)
        {
            var data = Encoding.UTF8.GetString(ea.Body.ToArray());
            var queueName = string.Empty;
            var result = false;

            await semaphore.WaitAsync().ConfigureAwait(false);
            
            try
            {
                queueName = ea.RoutingKey.Split('.')[0];
                var exchange = Enum.Parse<QueueExchanges>(ea.Exchange, true);
                result = await getActionResult(exchange, ea.RoutingKey, data);
            }
            catch (Exception exception)
            {
                logger.LogError(nameof(SubscribeQueue), exception);
            }

            semaphore.Release();

            if (queuesWithConfirm.Contains(queueName) && result)
                channel.BasicAck(ea.DeliveryTag, false);
        }
    }
}