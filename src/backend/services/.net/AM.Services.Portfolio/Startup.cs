using AM.Services.Common.Contracts.Helpers;
using AM.Services.Common.Contracts.SqlAccess;
using AM.Services.Portfolio.Background.Services;
using AM.Services.Portfolio.Background.ServiceTaskHandlers;
using AM.Services.Portfolio.Clients;
using AM.Services.Portfolio.Domain.DataAccess;
using AM.Services.Portfolio.Domain.DataAccess.RepositoryHandlers;
using AM.Services.Portfolio.Domain.Entities;
using AM.Services.Portfolio.Domain.Entities.Catalogs;
using AM.Services.Portfolio.Services.Entity;
using AM.Services.Portfolio.Services.RabbitMq.Function.Processes;
using AM.Services.Portfolio.Services.RabbitMq.Sync.Processes;
using AM.Services.Portfolio.Settings;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Polly;

using Shared.Contracts.Settings;
using Shared.Contracts.Settings.Sections;
using Shared.Core.Abstractions.Background.EntityState;
using Shared.Core.Background.EntityState;
using Shared.Core.Settings;
using Shared.Infrastructure.Extensions;

using System;

namespace AM.Services.Portfolio;

public class Startup
{
    private IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<ServiceSettings>(Configuration.GetSection(nameof(ServiceSettings)));

        services.AddMemoryCache();

        services.AddDbContext<DatabaseContext>(provider =>
        {
            provider.UseLazyLoadingProxies();
            provider.UseNpgsql(Configuration["ServiceSettings:ConnectionStrings:Db"]);
        });

        services.AddHttpClient<MoexClient>()
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
            .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(3, TimeSpan.FromSeconds(30)));

        services.AddControllers().AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.Converters.Add(new JsonHelper.TimeOnlyConverter());
            x.JsonSerializerOptions.Converters.Add(new JsonHelper.DateOnlyConverter());
        });

        services.Configure<RabbitMqConsumerSection>(Configuration.GetSection(RabbitMqConsumerSection.SectionName));
        services.Configure<BackgroundTaskSection>(Configuration.GetSection(BackgroundTaskSection.SectionName));
        services.AddRabbitMq();


        services.AddScoped(typeof(Repository<>));
        services.AddScoped<RepositoryHandler<User>, UserRepositoryHandler>();
        services.AddScoped<RepositoryHandler<Account>, AccountRepositoryHandler>();
        services.AddScoped<RepositoryHandler<Provider>, ProviderRepositoryHandler>();
        services.AddScoped<RepositoryHandler<Deal>, DealRepositoryHandler>();
        services.AddScoped<RepositoryHandler<Expense>, ExpenseRepositoryHandler>();
        services.AddScoped<RepositoryHandler<Income>, IncomeRepositoryHandler>();
        services.AddScoped<RepositoryHandler<Event>, EventRepositoryHandler>();
        services.AddScoped<RepositoryHandler<EventType>, EventTypeRepositoryHandler>();
        services.AddScoped<RepositoryHandler<Report>, ReportRepositoryHandler>();
        services.AddScoped<RepositoryHandler<Asset>, AssetRepositoryHandler>();
        services.AddScoped<RepositoryHandler<Derivative>, DerivativeRepositoryHandler>();


        services.AddScoped<AssetService>();
        services.AddScoped<DealService>();
        services.AddScoped<EventService>();
        services.AddScoped<DerivativeService>();

        services.AddTransient<AssetProcess>();
        services.AddTransient<DealProcess>();
        services.AddTransient<EventProcess>();
        services.AddTransient<DerivativeProcess>();

        services.AddTransient(typeof(EntityStateTask<>));
        services.AddTransient<IEntityStateBackgroundTaskHandler<ReportFile, BackgroundTaskSettings>, ReportFileTaskHandler>();
        
        services.AddHostedService<ReportFileBackgroundService>();
        services.AddHostedService<RabbitMqConsumerIncomingBackgroundService>();
        services.AddHostedService<RabbitMqConsumerSyncBackgroundService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseRouting();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}