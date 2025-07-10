using AM.Portfolio.Core.Abstractions.Documents.Excel;
using AM.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Bcs.Companies;
using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Bcs.Transactions;
using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Raiffeisen.Serbia.Transactions;
using AM.Portfolio.Core.Abstractions.Web.Http;
using AM.Portfolio.Core.Services.DataHeapServices.Bcs.Companies;
using AM.Portfolio.Core.Services.DataHeapServices.Bcs.Transactions;
using AM.Portfolio.Core.Services.DataHeapServices.Raiffeisen.Serbia.Transactions;
using AM.Portfolio.Infrastructure.Documents.ExcelDocumentServices;
using AM.Portfolio.Infrastructure.Persistence.Contexts;
using AM.Portfolio.Infrastructure.Persistence.Repositories;
using AM.Portfolio.Infrastructure.Settings;
using AM.Portfolio.Infrastructure.Web.Http;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Net.Shared.Documents.Abstractions.Excel;
using Net.Shared.Documents.Excel;
using Net.Shared.Models.Settings;
using Net.Shared.Persistence;
using Net.Shared.Queues.Abstractions.Core.WorkQueue;
using Net.Shared.Queues.WorkQueue;

using Polly;

namespace AM.Portfolio.Infrastructure;

public static partial class Registrations
{
    public static ILoggingBuilder AddSeq(this ILoggingBuilder builder, IConfiguration configuration)
    {
        var seqConnection = configuration.GetSection(SeqConnection.Name).Get<SeqConnection>();
        builder.ClearProviders();
        builder.AddSeq(seqConnection?.ConnectionString);
        return builder;
    }
    public static void AddPortfolioWorkerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPortfolioPersistence(configuration);

        services.AddPortfolioCore(configuration);
    }
    public static void AddPortfolioApiInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();

        services.AddPortfolioPersistence(configuration);

        services.AddPortfolioCore(configuration);

        services
            .AddOptions<WebclientConnectionSection>()
            .Bind(configuration.GetSection(WebclientConnectionSection.Name));

        services.AddHttpClient<IMoexHttpClient, MoexHttpclient>(nameof(MoexHttpclient))
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
            .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(3, TimeSpan.FromSeconds(30)));
    }
}

public static partial class Registrations
{
    private static void AddPortfolioCore(this IServiceCollection services, IConfiguration configuration)
    {
        #region SETTINGS
        services
            .AddOptions<HostSettings>()
            .Bind(configuration.GetSection(HostSettings.Name));
        #endregion

        #region LIBRARIES
        services.AddTransient<IWorkQueue, WorkQueue>();
        services.AddTransient<IExcelDocumentService, ExcelDataReaderService>();
        #endregion

        #region BCS SERVICES
        services.AddTransient<IBcsCompaniesParser, BcsCompaniesParser>();
        services.AddTransient<IBcsCompaniesMapper, BcsCompaniesMapper>();
        services.AddTransient<IBcsCompaniesHandler, BcsCompaniesHandler>();

        services.AddTransient<IBcsTransactionsParser, BcsTransactionsParser>();
        services.AddTransient<IBcsTransactionsMapper, BcsTransactionsMapper>();
        services.AddTransient<IBcsTransactionsHandler, BcsTransactionsHandler>();
        #endregion

        #region RAIFFEISEN SRB SERVICES
        services.AddTransient<IRaiffeisenSrbTransactionsParser, RaiffeisenSrbTransactionsParser>();
        services.AddTransient<IRaiffeisenSrbTransactionsMapper, RaiffeisenSrbTransactionsMapper>();
        services.AddTransient<IRaiffeisenSrbTransactionsHandler, RaiffeisenSrbTransactionsHandler>();
        #endregion

        services.AddTransient<IPortfolioExcelDocumentService, PortfolioExcelDocumentService>();
    }
    private static void AddPortfolioPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        #region SETTINGS
        services
            .AddOptions<DatabaseConnectionSection>()
            .Bind(configuration.GetSection(DatabaseConnectionSection.Name));
        #endregion

        #region STORAGES
        services.AddMongoDb<MongoDbPortfolioContext>(ServiceLifetime.Transient);
        services.AddPostgreSql<PostgreSqlPortfolioContext>(ServiceLifetime.Transient);
        #endregion

        #region APPS REPOSITORIES
        services.AddTransient<ICatalogRepository, CatalogRepository>();
        services.AddTransient<IDataHeapRepository, DataHeapRepository>();
        services.AddTransient<IAccountRepository, AccountRepository>();
        services.AddTransient<IAssetRepository, AssetRepository>();
        services.AddTransient<IDerivativeRepository, DerivativeRepository>();
        services.AddTransient<IDealRepository, DealRepository>();
        services.AddTransient<IEventRepository, EventRepository>();
        #endregion
    }
}
