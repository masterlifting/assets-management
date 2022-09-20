using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Services.EntityStateService.PipelineHandlers;
using AM.Services.Portfolio.Host.Services.Background.EntityState;
using AM.Services.Portfolio.Infrastructure.External.Webclients;
using AM.Services.Portfolio.Infrastructure.Persistence;
using AM.Services.Portfolio.Infrastructure.Persistence.Repositories;
using AM.Services.Portfolio.Infrastructure.Settings;

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
using Shared.Persistense.Entities.EntityState;
using Shared.Persistense.Handling.EntityState;

using System;


namespace AM.Services.Portfolio.Host;

public class Startup
{
    private IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<BackgroundTaskSection>(Configuration.GetSection(BackgroundTaskSection.Name));
        services.Configure<DatabaseConnectionSection>(Configuration.GetSection(DatabaseConnectionSection.Name));
        services.Configure<WebclientConnectionSection>(Configuration.GetSection(WebclientConnectionSection.Name));

        services.AddMemoryCache();

        services.AddDbContext<DatabaseContext>(provider =>
        {
            var dbConnections = Configuration.GetValue<DatabaseConnectionSection>(DatabaseConnectionSection.Name);
            provider.UseLazyLoadingProxies();
            provider.UseNpgsql(dbConnections.Postgres.GetConnectionString());
        });

        services.AddHttpClient<MoexWebclient>()
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
            .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(3, TimeSpan.FromSeconds(30)));

        services.AddControllers().AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.Converters.Add(new JsonExtensions.TimeOnlyConverter());
            x.JsonSerializerOptions.Converters.Add(new JsonExtensions.DateOnlyConverter());
        });

        services.AddRabbitMq(Configuration);

        services.AddTransient<ICatalogRepository<Step>, CatalogRepository<Step, DatabaseContext>>();
        services.AddTransient<IReportRepository, ReportRepository<DatabaseContext>>();
        services.AddTransient<IUserRepository, UserRepository<DatabaseContext>>();

        services.AddTransient(typeof(EntityStateHandler<>));

        services.AddTransient<IEntityStatePipelineHandler<Asset>, AssetPipelineHandler>();
        services.AddTransient<IEntityStatePipelineHandler<Derivative>, DerivativePipelineHandler>();
        services.AddTransient<IEntityStatePipelineHandler<Deal>, DealPipelineHandler>();
        services.AddTransient<IEntityStatePipelineHandler<Event>, EventPipelineHandler>();
        services.AddTransient<IEntityStatePipelineHandler<Report>, ReportPipelineHandler>();

        services.AddHostedService<AssetBackgroundService>();
        services.AddHostedService<DerivativeBackgroundService>();
        services.AddHostedService<DealBackgroundService>();
        services.AddHostedService<EventBackgroundService>();
        services.AddHostedService<ReportBackgroundService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseRouting();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}