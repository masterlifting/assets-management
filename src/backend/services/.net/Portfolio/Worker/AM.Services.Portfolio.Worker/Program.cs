using AM.Services.Portfolio.Infrastructure.Persistence.Contexts;
using AM.Services.Portfolio.Infrastructure.Settings;
using AM.Services.Portfolio.Worker.BackgroundServices;
using AM.Services.Portfolio.Worker.BackgroundTasks;

using Shared.Background.Settings.Sections;
using Shared.Persistense.Abstractions.Repositories;
using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Worker;

public class Program
{
    public static void Main(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            var configuration = hostContext.Configuration;

            services.Configure<BackgroundTaskSection>(configuration.GetSection(BackgroundTaskSection.Name));
            services.Configure<DatabaseConnectionSection>(configuration.GetSection(DatabaseConnectionSection.Name));

            services.AddScoped<PostgreSQLPortfolioContext>();
            services.AddScoped<IPostgreSQLRepository, PostgreSQLRepository>();
            
            services.AddScoped<MongoDBPortfolioContext>();
            services.AddScoped<IMongoDBRepository, MongoDBRepository>();

            services.AddHostedService<ProcessingDataAsBytesBackgroundService>();
            services.AddTransient<ProcessingDataAsBytesBackgroundTask>();
            
            services.AddHostedService<ProcessingDataAsJsonBackgroundService>();
            services.AddTransient<ProcessingDataAsJsonBackgroundTask>();
        })
        .Build()
        .Run();
}