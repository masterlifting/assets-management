using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Shared.Queue.Abstractions.Connection;
using Shared.Queue.Domain.RabbitMq.Connection;
using Shared.Queue.Settings.RabbitMq;

namespace Shared.Queue;

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
