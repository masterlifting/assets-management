using AM.Services.Portfolio.Core.Abstractions.Excel;
using AM.Services.Portfolio.Core.Abstractions.Web;
using AM.Services.Portfolio.Core.Services.BcsServices.Implementations.v1;
using AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;
using AM.Services.Portfolio.Infrastructure.Excel;
using AM.Services.Portfolio.Infrastructure.Persistence.Contexts;
using AM.Services.Portfolio.Infrastructure.Settings;
using AM.Services.Portfolio.Infrastructure.Web;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Polly;

using Shared.Persistense.Abstractions.Repositories;

using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure;

public static class PortfolioInfrastructureRegistration
{
    public static void AddPortfolioCoreServices(this IServiceCollection services)
    {
        services.AddTransient<IPortfolioExcelService, PortfolioExcelService>();
        services.AddTransient<IBcsReportJsonToEntitiesService, BcsReportJsonToEntitiesService>();
        services.AddTransient<IBcsReportDataToJsonService, BcsReportDataToJsonService>();
    }
    public static void AddPortfolioPersistance(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseConnectionSection>(configuration.GetSection(DatabaseConnectionSection.Name));

        services.AddScoped<PostgreSQLPortfolioContext>();
        services.AddScoped<IPostgreSQLRepository, PostgreSQLRepository>();

        services.AddScoped<MongoDBPortfolioContext>();
        services.AddScoped<IMongoDBRepository, MongoDBRepository>();
    }
    public static void AddPortfolioHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<WebclientConnectionSection>(configuration.GetSection(WebclientConnectionSection.Name));

        services.AddHttpClient<IMoexWebclient, MoexWebclient>()
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
            .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(3, TimeSpan.FromSeconds(30)));
    }
}
