using AM.Services.Portfolio.Core.Abstractions.ExcelService;
using AM.Services.Portfolio.Core.Abstractions.WebServices;
using AM.Services.Portfolio.Core.Services.BcsServices.Implementations.v1;
using AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;
using AM.Services.Portfolio.Infrastructure.ExcelServices;
using AM.Services.Portfolio.Infrastructure.Persistence.Contexts;
using AM.Services.Portfolio.Infrastructure.Settings;
using AM.Services.Portfolio.Infrastructure.WebClients;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Polly;

using Shared.Persistence.Abstractions.Repositories;
using Shared.Persistence.Repositories;

namespace AM.Services.Portfolio.Infrastructure;

public static class PortfolioInfrastructureRegistration
{
    public static void AddPortfolioCoreServices(this IServiceCollection services)
    {
        services.AddTransient<IPortfolioExcelService, PortfolioExcelService>();
        services.AddTransient<IBcsReportService, BcsReportService>();
    }
    public static void AddPortfolioPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseConnectionSection>(configuration.GetSection(DatabaseConnectionSection.Name));

        services.AddScoped<PostgreSQLPortfolioContext>();
        services.AddScoped<IPostgreSQLRepository, PostgreSQLRepository<PostgreSQLPortfolioContext>>();

        services.AddScoped<MongoDBPortfolioContext>();
        services.AddScoped<IMongoDBRepository, MongoDBRepository<MongoDBPortfolioContext>>();
    }
    public static void AddPortfolioHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<WebclientConnectionSection>(configuration.GetSection(WebclientConnectionSection.Name));

        services.AddHttpClient<IMoexWebclient, MoexWebclient>()
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
            .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(3, TimeSpan.FromSeconds(30)));
    }
}
