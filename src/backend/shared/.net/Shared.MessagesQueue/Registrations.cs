using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.MessagesQueue.Abstractions.Connection;
using Shared.MessagesQueue.Domain.RabbitMq.Connection;
using Shared.MessagesQueue.Settings.RabbitMq;

namespace Shared.MessagesQueue;

public static class Registrations
{
    public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSection>(_ => configuration.GetSection(RabbitMqSection.Name));
        services.AddSingleton<RabbitMqClient>();
        services.AddTransient<IMqConsumer, RabbitMqConsumer>();
        services.AddTransient<IMqProducer, RabbitMqProducer>();
    }
}