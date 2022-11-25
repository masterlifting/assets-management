using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Abstractions.Web;
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

using Shared.Extensions.Serialization;
using Shared.Persistense.Repositories;

using System;

namespace AM.Services.Portfolio.API;

public sealed class Startup
{
    private IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        var databaseSection = Configuration.GetSection(DatabaseConnectionSection.Name);
        var webclientSection = Configuration.GetSection(WebclientConnectionSection.Name);
        
        services.Configure<DatabaseConnectionSection>(databaseSection);
        services.Configure<WebclientConnectionSection>(webclientSection);

        services.AddMemoryCache();

        services.AddDbContext<DatabaseContext>(provider =>
        {
            var dbConnection = databaseSection.Get<DatabaseConnectionSection>().Postgres;
            provider.UseNpgsql(dbConnection.GetConnectionString());
        }, ServiceLifetime.Transient);

        services.AddHttpClient<IMoexWebclient, MoexWebclient>()
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
            .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(3, TimeSpan.FromSeconds(30)));

        services.AddControllers().AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.Converters.Add(new JsonExtensions.TimeOnlyConverter());
            x.JsonSerializerOptions.Converters.Add(new JsonExtensions.DateOnlyConverter());
        });

        services.AddRabbitMq(Configuration);

        services.AddScoped(typeof(CatalogRepository<,>));
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


    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseRouting();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}