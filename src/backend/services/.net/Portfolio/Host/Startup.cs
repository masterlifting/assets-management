using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Abstractions.Web;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Services.EntityState.Handlers;
using AM.Services.Portfolio.Host.Services.Background.EntityState;
using AM.Services.Portfolio.Infrastructure.Persistence.Context;
using AM.Services.Portfolio.Infrastructure.Persistence.Repositories;
using AM.Services.Portfolio.Infrastructure.Settings;
using AM.Services.Portfolio.Infrastructure.Web;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Polly;

using Shared.Background.Settings.Sections;
using Shared.Extensions.Serialization;
using Shared.MessagesQueue;
using Shared.Persistense.Abstractions.Handling.EntityState;
using Shared.Persistense.Handling.EntityState;

using System;

namespace AM.Services.Portfolio.Host
{
    public sealed class Startup
    {
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            var dbSection = Configuration.GetSection(DatabaseConnectionSection.Name);

            services.Configure<DatabaseConnectionSection>(dbSection);
            services.Configure<BackgroundTaskSection>(Configuration.GetSection(BackgroundTaskSection.Name));
            services.Configure<WebclientConnectionSection>(Configuration.GetSection(WebclientConnectionSection.Name));

            services.AddMemoryCache();

            services.AddDbContext<DatabaseContext>(provider =>
            {
                var dbConnection = dbSection.Get<DatabaseConnectionSection>().Postgres;
                provider.UseNpgsql(dbConnection.GetConnectionString());
            });

            services.AddHttpClient<IMoexWebclient, MoexWebclient>()
                .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(3, TimeSpan.FromSeconds(30)));

            services.AddControllers().AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.Converters.Add(new JsonExtensions.TimeOnlyConverter());
                x.JsonSerializerOptions.Converters.Add(new JsonExtensions.DateOnlyConverter());
            });

            services.AddRabbitMq(Configuration);

            services.AddTransient(typeof(CatalogRepository<,>));
            services.AddTransient<IUserRepository, UserRepository<DatabaseContext>>();
            services.AddTransient<IAccountRepository, AccountRepository<DatabaseContext>>();
            services.AddTransient<IAssetRepository, AssetRepository<DatabaseContext>>();
            services.AddTransient<IDerivativeRepository, DerivativeRepository<DatabaseContext>>();
            services.AddTransient<IDealRepository, DealRepository<DatabaseContext>>();
            services.AddTransient<IEventRepository, EventRepository<DatabaseContext>>();
            services.AddTransient<IExpenseRepository, ExpenseRepository<DatabaseContext>>();
            services.AddTransient<IIncomeRepository, IncomeRepository<DatabaseContext>>();
            services.AddTransient<IReportDataRepository, ReportDataRepository<DatabaseContext>>();
            services.AddTransient<IReportRepository, ReportRepository<DatabaseContext>>();

            services.AddTransient(typeof(EntityStateHandler<>));

            services.AddTransient<IEntityStateHandler<Asset>, AssetStateHandler>();
            services.AddTransient<IEntityStateHandler<Derivative>, DerivativeStateHandler>();
            services.AddTransient<IEntityStateHandler<Deal>, DealStateHandler>();
            services.AddTransient<IEntityStateHandler<Event>, EventStateHandler>();
            services.AddTransient<IEntityStateHandler<ReportData>, ReportDataStateHandler>();

            services.AddHostedService<AssetBackgroundService>();
            services.AddHostedService<DerivativeBackgroundService>();
            services.AddHostedService<DealBackgroundService>();
            services.AddHostedService<EventBackgroundService>();
            services.AddHostedService<ReportDataBackgroundService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}