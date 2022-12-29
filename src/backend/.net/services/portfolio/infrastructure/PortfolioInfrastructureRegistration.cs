﻿using AM.Services.Portfolio.Core.Abstractions.ExcelService;
using AM.Services.Portfolio.Core.Abstractions.Persistence;
using AM.Services.Portfolio.Core.Abstractions.WebServices;
using AM.Services.Portfolio.Core.Domain.Persistence.Collections;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;
using AM.Services.Portfolio.Core.Services.BcsServices.Implementations.v1;
using AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;
using AM.Services.Portfolio.Infrastructure.ExcelServices;
using AM.Services.Portfolio.Infrastructure.Persistence;
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
        
        services.AddScoped<PostgrePortfolioContext>();
        services.AddScoped<MongoPortfolioContext>();

        services.AddScoped<IPersistenceNoSqlRepository<IncomingData>, MongoRepository<IncomingData, MongoPortfolioContext>>();
        services.AddScoped<IPersistenceSqlRepository<ProcessStep>, PostgreRepository<ProcessStep, PostgrePortfolioContext>>();
        services.AddScoped<IPersistenceSqlRepository<Asset>, PostgreRepository<Asset, PostgrePortfolioContext>>();
        services.AddScoped<IPersistenceSqlRepository<Deal>, PostgreRepository<Deal, PostgrePortfolioContext>>();
        services.AddScoped<IPersistenceSqlRepository<Event>, PostgreRepository<Event, PostgrePortfolioContext>>();
        services.AddScoped<IPersistenceSqlRepository<Derivative>, PostgreRepository<Derivative, PostgrePortfolioContext>>();
        services.AddScoped<IPersistenceSqlRepository<User>, PostgreRepository<User, PostgrePortfolioContext>>();

        services.AddScoped<IUnitOfWorkRepository, UnitOfWorkRepository>();
    }
    public static void AddPortfolioHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<WebclientConnectionSection>(configuration.GetSection(WebclientConnectionSection.Name));

        services.AddHttpClient<IMoexWebclient, MoexWebclient>()
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
            .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(3, TimeSpan.FromSeconds(30)));
    }
}