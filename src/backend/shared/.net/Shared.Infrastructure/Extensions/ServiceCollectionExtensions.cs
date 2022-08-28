using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Abstractions.Queues;
using Shared.Infrastructure.RabbitMq;

namespace Shared.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddRabbitMq(this IServiceCollection services)
    {
        services.AddSingleton<RabbitMqClient>();
        services.AddTransient<IMqConsumer, RabbitMqConsumer>();
        services.AddTransient<IMqProducer, RabbitMqProducer>();
    }
}