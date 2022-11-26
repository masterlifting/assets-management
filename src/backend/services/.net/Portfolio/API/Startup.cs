using AM.Services.Portfolio.Core.Abstractions.Web;
using AM.Services.Portfolio.Infrastructure.Persistence.Contexts;
using AM.Services.Portfolio.Infrastructure.Settings;
using AM.Services.Portfolio.Infrastructure.Web;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Polly;

using Shared.Extensions.Serialization;
using Shared.Persistense.Abstractions.Repositories;
using Shared.Persistense.Repositories;

using System;

namespace AM.Services.Portfolio.API;

public sealed class Startup
{
    private IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMemoryCache();

        services.Configure<DatabaseConnectionSection>(Configuration.GetSection(DatabaseConnectionSection.Name));
        services.Configure<WebclientConnectionSection>(Configuration.GetSection(WebclientConnectionSection.Name));

        services.AddScoped<PostgreSQLPortfolioContext>();
        services.AddScoped<IPostgreSQLRepository, PostgreSQLRepository>();

        services.AddScoped<MongoDBPortfolioContext>();
        services.AddScoped<IMongoDBRepository, MongoDBRepository>();

        services.AddHttpClient<IMoexWebclient, MoexWebclient>()
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
            .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(3, TimeSpan.FromSeconds(30)));

        services.AddControllers().AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.Converters.Add(new JsonExtensions.TimeOnlyConverter());
            x.JsonSerializerOptions.Converters.Add(new JsonExtensions.DateOnlyConverter());
        });

        //services.AddRabbitMq(Configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseRouting();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}