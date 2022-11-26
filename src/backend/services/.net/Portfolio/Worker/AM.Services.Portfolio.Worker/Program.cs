using AM.Services.Portfolio.Infrastructure.Persistence.Context;
using AM.Services.Portfolio.Infrastructure.Settings;
using AM.Services.Portfolio.Worker.BackgroundServices;
using AM.Services.Portfolio.Worker.BackgroundTaskStepHandlers;

using Microsoft.EntityFrameworkCore;

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

            var backgroundTaskSection = configuration.GetSection(BackgroundTaskSection.Name);
            var databaseSection = configuration.GetSection(DatabaseConnectionSection.Name);

            services.Configure<BackgroundTaskSection>(backgroundTaskSection);
            services.Configure<DatabaseConnectionSection>(databaseSection);

            services.AddDbContext<DatabaseContext>(provider =>
            {
                var dbConnection = databaseSection.Get<DatabaseConnectionSection>().Postgres;
                provider.UseNpgsql(dbConnection.GetConnectionString());
            }, ServiceLifetime.Transient);


            services.AddTransient<IPostgresqlRepository, PostgresqlRepository>();
            services.AddTransient<IMongoDBRepository, MongoDBRepository>();

            services.AddHostedService<EntitiesProcessingDataAsBytesBackgroundService>();
            services.AddTransient<EntitiesProcessingDataAsBytesBackgroundTask>();
            
            services.AddHostedService<EntitiesProcessingDataAsJsonBackgroundService>();
            services.AddTransient<EntitiesProcessingDataAsJsonBackgroundTask>();
        })
        .Build()
        .Run();
}