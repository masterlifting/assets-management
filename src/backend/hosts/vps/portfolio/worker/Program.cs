using AM.Services.Portfolio.Core.Domain.Persistence.Collections;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;
using AM.Services.Portfolio.Infrastructure;
using AM.Services.Portfolio.Worker.BackgroundServices;
using AM.Services.Portfolio.Worker.BackgroundTasks;

using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Settings.Sections;

namespace AM.Services.Portfolio.Worker;

public class Program
{
    public static void Main(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            var configuration = hostContext.Configuration;

            services.AddPortfolioPersistence(configuration);
            services.AddPortfolioCoreServices();

            services.Configure<BackgroundTaskSection>(configuration.GetSection(BackgroundTaskSection.Name));

            services.AddHostedService<BackgroundServiceIncomingDataProcessing>();
            services.AddTransient<BackgroundTaskProcessing<IncomingData, ProcessStep>, BackgroundTaskIncomingDataProcessing>();
        })
        .Build()
        .Run();
}