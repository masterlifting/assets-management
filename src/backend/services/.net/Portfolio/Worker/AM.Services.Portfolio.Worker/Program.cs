using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;
using AM.Services.Portfolio.Infrastructure.Persistence.Context;
using AM.Services.Portfolio.Infrastructure.Persistence.Repositories;
using AM.Services.Portfolio.Infrastructure.Settings;
using AM.Services.Portfolio.Worker.BackgroundServices;
using AM.Services.Portfolio.Worker.BackgroundTaskStepHandlers;

using Microsoft.EntityFrameworkCore;

using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Handlers;
using Shared.Background.Settings.Sections;
using Shared.Persistense.Abstractions.Entities.Catalogs;
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


            services.AddScoped<ICatalogRepository<IProcessingStep>, CatalogRepository<ProcessStep, DatabaseContext>>();
            //services.AddScoped(typeof(CatalogRepository<,>));

            services.AddTransient<IUserRepository, UserRepository<DatabaseContext>>();
            services.AddTransient<IAccountRepository, AccountRepository<DatabaseContext>>();
            services.AddTransient<IAssetRepository, AssetRepository<DatabaseContext>>();
            services.AddTransient<IDerivativeRepository, DerivativeRepository<DatabaseContext>>();
            services.AddTransient<IDealRepository, DealRepository<DatabaseContext>>();
            services.AddTransient<IEventRepository, EventRepository<DatabaseContext>>();
            services.AddTransient<IExpenseRepository, ExpenseRepository<DatabaseContext>>();
            services.AddTransient<IIncomeRepository, IncomeRepository<DatabaseContext>>();

            //services.AddTransient<BackgroundTaskStepHandler<Asset>, AssetStateHandler>();
            //services.AddTransient<BackgroundTaskStepHandler<Deal>, DealStateHandler>();
            //services.AddTransient<BackgroundTaskStepHandler<Derivative>, DerivativeStateHandler>();
            //services.AddTransient<BackgroundTaskStepHandler<Event>, EventStateHandler>();
            services.AddTransient<BackgroundTaskStepHandler<DataAsBytes>, DataAsBytesStepHandler>();
            services.AddTransient<BackgroundTaskStepHandler<DataAsJson>, DataAsJsonStepHandler>();

            services.AddTransient(typeof(EntitiesProcessingBackgroundTask<>));

            services.AddHostedService<AssetBackgroundService>();
            services.AddHostedService<DerivativeBackgroundService>();
            services.AddHostedService<DealBackgroundService>();
            services.AddHostedService<EventBackgroundService>();
            services.AddHostedService<DataAsBytesBackgroundService>();
            services.AddHostedService<DataAsJsonBackgroundService>();
        })
        .Build()
        .Run();
}