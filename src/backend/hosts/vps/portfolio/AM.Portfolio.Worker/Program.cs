using AM.Portfolio.Infrastructure;
using AM.Portfolio.Worker.Background.Services;

using Net.Shared.Background.Abstractions;
using Net.Shared.Background.ConfigurationProviders;
using Net.Shared.Background.Models.Settings;

namespace AM.Portfolio.Worker;

public static class Program
{
    public static void Main(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureLogging((hostContext, logging) => logging.AddSeq(hostContext.Configuration))
        .ConfigureServices((hostContext, services) =>
        {
            var configuration = hostContext.Configuration;

            services.AddPortfolioWorkerInfrastructure(configuration);

            #region BACKGROUND SERVICE
            services
                .AddOptions<BackgroundTasksConfiguration>()
                .Bind(configuration.GetSection(BackgroundTasksConfiguration.Name));

            services.AddSingleton<IBackgroundServiceConfigurationProvider, BackgroundServiceOptionsProvider>();

            #region TASKS
            services.AddHostedService<DataHeapBackgroundService>();
            services.AddHostedService<EventsBackgroundService>();
            #endregion
            
            #endregion
        })
        .Build()
        .Run();
}
