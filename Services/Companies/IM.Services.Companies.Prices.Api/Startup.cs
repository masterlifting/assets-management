using IM.Services.Companies.Prices.Api.DataAccess;
using IM.Services.Companies.Prices.Api.DataAccess.Entities;
using IM.Services.Companies.Prices.Api.DataAccess.Repository;
using IM.Services.Companies.Prices.Api.Services.Agregators.Implementations;
using IM.Services.Companies.Prices.Api.Services.Agregators.Interfaces;
using IM.Services.Companies.Prices.Api.Services.Background.PriceUpdaterBackgroundServices.Implementations;
using IM.Services.Companies.Prices.Api.Services.Background.PriceUpdaterBackgroundServices.Interfaces;
using IM.Services.Companies.Prices.Api.Services.Background.RabbitMqBackgroundServices;
using IM.Services.Companies.Prices.Api.Services.HealthCheck;
using IM.Services.Companies.Prices.Api.Services.Mapper;
using IM.Services.Companies.Prices.Api.Settings;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Polly;

using System;

namespace IM.Services.Companies.Prices.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ServiceSettings>(Configuration.GetSection(nameof(ServiceSettings)));

            services.AddDbContext<PricesContext>(provider =>
            {
                provider.UseLazyLoadingProxies();
                //todo: connection string to improve
                provider.UseNpgsql(Configuration["ServiceSettings:ConnectionStrings:Db"]);
            });

            services.AddControllers();

            services.AddHttpClient<TdAmeritradeClient>()
                .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(10, retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp))))
                .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(3, TimeSpan.FromSeconds(10)));
            services.AddHttpClient<MoexClient>()
                .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(10, retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp))))
                .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(3, TimeSpan.FromSeconds(10)));

            services.AddHealthChecks()
                .AddCheck<TdAmeritradeHealthCheck>("TdAmeritrade")
                .AddCheck<MoexHealthCheck>("Moex");

            services.AddScoped<IPriceMapper, PriceMapper>();
            services.AddScoped<IPricesDtoAgregator, PricesDtoAgregator>();
            services.AddScoped<IPriceUpdater, PriceUpdater>();

            services.AddScoped(typeof(EntityRepository<>));
            services.AddScoped<IEntityChecker<Ticker>, TckerChecker>();

            services.AddHostedService<RabbitmqBackgroundService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
