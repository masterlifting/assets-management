using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Shared.MessagesQueue.Implementations.RabbitMq.Settings.Sections;
using Shared.MessagesQueue.Interfaces;

namespace Shared.MessagesQueue.Implementations.RabbitMq.Registration;

public static class ServiceCollectionExtension
{
    public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSection>(_ => configuration.GetSection(RabbitMqSection.Name));
        services.AddSingleton<RabbitMqClient>();
        services.AddTransient<IMqConsumer, RabbitMqConsumer>();
        services.AddTransient<IMqProducer, RabbitMqProducer>();
    }
}