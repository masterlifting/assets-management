using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

using Shared.Core.Constants;
using Shared.Core.Domains.RabbitMq;
using Shared.Infrastructure.RabbitMq.Settings;
using static Shared.Core.Extensions.LoggerExtensions;

namespace Shared.Infrastructure.RabbitMq;

public sealed class RabbitMqClient
{
    private readonly string _initiator;

    private readonly ILogger<RabbitMqClient> _logger;
    private readonly ConnectionFactory _connectionFactory;
    public RabbitMqClient(ILogger<RabbitMqClient> logger, IOptions<RabbitMqClientSettings> options)
    {
        _logger = logger;

        var settings = options.Value;
        _connectionFactory = new ConnectionFactory
        {
            HostName = settings.Host,
            UserName = settings.User,
            Password = settings.Password
        };

        var objectId = GetHashCode();
        _initiator = $"RabbitMQ Client {objectId}";

        _logger.LogTrace(_initiator, CoreConstants.Actions.Connect, CoreConstants.Results.Start);

        using var connection = _connectionFactory.CreateConnection();
        using var model = connection.CreateModel();

        _logger.LogTrace(_initiator, CoreConstants.Actions.Connect, CoreConstants.Results.Success);

        _logger.LogTrace(_initiator, "Register models", CoreConstants.Results.Start);

        foreach (var item in settings.ModelBuilders)
            CreateModel(model, item.Exchange, item.Queue);

        _logger.LogTrace(_initiator, "Register models", CoreConstants.Results.Success);
    }

    public IModel CreateModel()
    {
        _logger.LogTrace(_initiator, CoreConstants.Actions.Connect, CoreConstants.Results.Start);

        using var connection = _connectionFactory.CreateConnection();
        var model = connection.CreateModel();

        _logger.LogTrace(_initiator, CoreConstants.Actions.Connect, CoreConstants.Results.Success);

        return model;
    }
    private static void CreateModel(IModel model, RabbitMqExchange exchange, RabbitMqQueue queue)
    {
        var exchangeType = string.Intern(exchange.Type.ToString());
        var exchangeName = string.Intern(exchange.Name.ToString());
        var routingKey = string.Intern($"{exchangeName}.{queue.Name}");

        model.ExchangeDeclare(exchangeName, exchangeType, exchange.IsDurable, exchange.IsAutoDelete, exchange.Arguments);
        model.QueueDeclare(queue.Name, queue.IsDurable, queue.IsExclusive, queue.IsAutoDelete, queue.Arguments);

        model.QueueBind(queue.Name, exchangeName, routingKey);
    }

    ~RabbitMqClient() => 
        _logger.LogTrace(_initiator, CoreConstants.Actions.Disconnect, CoreConstants.Results.Success);
}