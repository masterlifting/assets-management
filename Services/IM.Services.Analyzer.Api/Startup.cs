using CommonServices.RabbitServices;

using IM.Services.Analyzer.Api.DataAccess;
using IM.Services.Analyzer.Api.DataAccess.Entities;
using IM.Services.Analyzer.Api.DataAccess.Repository;
using IM.Services.Analyzer.Api.Services.BackgroundServices;
using IM.Services.Analyzer.Api.Services.DtoServices;
using IM.Services.Analyzer.Api.Services.RabbitServices;
using IM.Services.Analyzer.Api.Settings;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IM.Services.Analyzer.Api
{
    public class Startup
    {
        private static readonly QueueExchanges[] targetExchanges = new[] { QueueExchanges.crud, QueueExchanges.calculator };
        private static readonly QueueNames[] targetQueues = new[]
        {
            QueueNames.companiesanalyzercrud
            ,QueueNames.companiesanalyzercalculator 
        };

        public Startup(IConfiguration configuration) => Configuration = configuration;
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ServiceSettings>(Configuration.GetSection(nameof(ServiceSettings)));

            services.AddDbContext<AnalyzerContext>(provider =>
            {
                provider.UseLazyLoadingProxies();
                provider.UseNpgsql(Configuration["ServiceSettings:ConnectionStrings:Db"]);
            });

            services.AddControllers();

            services.AddScoped<CoefficientDtoAgregator>();
            services.AddScoped<RatingDtoAgregator>();
            services.AddScoped<RecommendationDtoAgregator>();

            services.AddScoped(typeof(EntityRepository<>));
            services.AddScoped<IEntityChecker<Ticker>, TckerChecker>();

            services.AddSingleton(x => new RabbitBuilder(
                Configuration["ServiceSettings:ConnectionStrings:Mq"]
                ,QueueConfiguration.GetConfiguredData(targetExchanges, targetQueues)));
            services.AddSingleton<RabbitService>();
            services.AddSingleton<RabbitActionService>();

            services.AddHostedService<RabbitBackgroundService>();
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
            });
        }
    }
}
