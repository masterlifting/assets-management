using AM.Services.Portfolio.API.Services;
using AM.Services.Portfolio.API.Services.Interfaces;
using AM.Services.Portfolio.Infrastructure;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Shared.Extensions.Serialization;

namespace AM.Services.Portfolio.API;

public sealed class Startup
{
    private IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddPortfolioPersistence(Configuration);
        services.AddPortfolioCoreServices();

        services.AddPortfolioHttpClients(Configuration);

        services.AddControllers().AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.Converters.Add(new JsonExtensions.TimeOnlyConverter());
            x.JsonSerializerOptions.Converters.Add(new JsonExtensions.DateOnlyConverter());
        });

        services.AddTransient<IReportApi, ReportApi>();

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