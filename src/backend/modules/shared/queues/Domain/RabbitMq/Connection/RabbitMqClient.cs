using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;

using Shared.Extensions.Logging;
using Shared.Queue;
using Shared.Queue.Domain.RabbitMq;
using Shared.Queue.Settings.RabbitMq;

namespace Shared.Queue.Domain.RabbitMq.Connection;

public sealed class RabbitMqClient
{
    private bool _isRegister;
    private readonly string _initiator;

    private readonly ILogger<RabbitMqClient> _logger;
    private readonly ConnectionFactory _connectionFactory;
    private readonly RabbitMqClientSettings _settings;

    public RabbitMqClient(ILogger<RabbitMqClient> logger, IOptions<RabbitMqClientSettings> options)
    {
        _logger = logger;
        _settings = options.Value;

        _connectionFactory = new ConnectionFactory
        {
            HostName = _settings.Host,
            UserName = _settings.User,
            Password = _settings.Password
        };

        var objectId = GetHashCode();
        _initiator = $"RabbitMq client {objectId}";
    }

    public IModel CreateModel()
    {
        if (!_isRegister)
            Register();

        _logger.LogTrace(_initiator, Constants.Actions.Connect, Constants.Actions.Start);

        using var connection = _connectionFactory.CreateConnection();
        var model = connection.CreateModel();

        _logger.LogTrace(_initiator, Constants.Actions.Connect, Constants.Actions.Success);

        return model;
    }

    private void Register()
    {
        using var model = CreateModel();

        _logger.LogTrace(_initiator, "Register models", Constants.Actions.Start);

        foreach (var item in _settings.ModelBuilders)
            Register(model, item.Exchange, item.Queue);

        _isRegister = true;

        _logger.LogTrace(_initiator, "Register models", Constants.Actions.Success);
    }
    private static void Register(IModel model, RabbitMqExchange exchange, RabbitMqQueue queue)
    {
        var exchangeType = string.Intern(exchange.Type.ToString());
        var exchangeName = string.Intern(exchange.Name.ToString());
        var routingKey = string.Intern($"{exchangeName}.{queue.Name}.*");

        model.ExchangeDeclare(exchangeName, exchangeType, exchange.IsDurable, exchange.IsAutoDelete, exchange.Arguments);
        model.QueueDeclare(queue.Name, queue.IsDurable, queue.IsExclusive, queue.IsAutoDelete, queue.Arguments);

        model.QueueBind(queue.Name, exchangeName, routingKey);
    }
}