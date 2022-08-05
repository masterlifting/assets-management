using AM.Services.Common.Contracts.Helpers;
using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Market.Domain.Entities;
using AM.Services.Market.Domain.Entities.ManyToMany;
using AM.Services.Market.Services.RabbitMq.Function.Processes;
using static AM.Services.Common.Contracts.Helpers.ServiceHelper;

namespace AM.Services.Market.Services.RabbitMq.Function;

public sealed class RabbitFunction : IRabbitAction
{
    private const string methodName = nameof(GetResultAsync);
    private readonly IServiceScopeFactory scopeFactory;
    public RabbitFunction(IServiceScopeFactory scopeFactory) => this.scopeFactory = scopeFactory;

    public Task GetResultAsync(QueueEntities entity, QueueActions action, string data)
    {
        var serviceProvider = scopeFactory.CreateAsyncScope().ServiceProvider;
        var logger = serviceProvider.GetRequiredService<ILogger<RabbitFunction>>();
        return entity switch
        {
            QueueEntities.AssetSource => Task.Run(() =>
            {
                var model = JsonHelper.Deserialize<CompanySource>(data);

                return TaskHelper.WhenAny(methodName, logger, new List<Task>
                    {
                        serviceProvider.GetRequiredService<FloatProcess>().ProcessAsync(action, model),
                        serviceProvider.GetRequiredService<PriceProcess>().ProcessAsync(action, model),
                        serviceProvider.GetRequiredService<ReportProcess>().ProcessAsync(action, model),
                        serviceProvider.GetRequiredService<SplitProcess>().ProcessAsync(action, model),
                        serviceProvider.GetRequiredService<DividendProcess>().ProcessAsync(action, model)
                    });
            }),
            QueueEntities.AssetSources => Task.Run(() =>
            {
                var models = JsonHelper.Deserialize<CompanySource[]>(data);

                return TaskHelper.WhenAny(methodName, logger, new List<Task>
                    {
                        serviceProvider.GetRequiredService<FloatProcess>().ProcessRangeAsync(action, models),
                        serviceProvider.GetRequiredService<PriceProcess>().ProcessRangeAsync(action, models),
                        serviceProvider.GetRequiredService<ReportProcess>().ProcessRangeAsync(action, models),
                        serviceProvider.GetRequiredService<SplitProcess>().ProcessRangeAsync(action, models),
                        serviceProvider.GetRequiredService<DividendProcess>().ProcessRangeAsync(action, models)
                    });
            }),
            QueueEntities.Price => Task.Run(() =>
             {
                 var model = JsonHelper.Deserialize<Price>(data);

                 return TaskHelper.WhenAny(methodName, logger, new List<Task>
                {
                        serviceProvider.GetRequiredService<PriceProcess>().ProcessAsync(action, model),
                        serviceProvider.GetRequiredService<CoefficientProcess>().ProcessAsync(action, model)
                });
             }),
            QueueEntities.Prices => Task.Run(() =>
            {
                var models = JsonHelper.Deserialize<Price[]>(data);

                return TaskHelper.WhenAny(methodName, logger, new List<Task>
                {
                        serviceProvider.GetRequiredService<PriceProcess>().ProcessRangeAsync(action, models),
                        serviceProvider.GetRequiredService<CoefficientProcess>().ProcessRangeAsync(action, models)
                });
            }),
            QueueEntities.Report => Task.Run(() =>
            {
                var model = JsonHelper.Deserialize<Report>(data);

                return TaskHelper.WhenAny(methodName, logger, new List<Task>
                {
                        serviceProvider.GetRequiredService<ReportProcess>().ProcessAsync(action, model),
                        serviceProvider.GetRequiredService<CoefficientProcess>().ProcessAsync(action, model)
                });
            }),
            QueueEntities.Reports => Task.Run(() =>
            {
                var models = JsonHelper.Deserialize<Report[]>(data);

                return TaskHelper.WhenAny(methodName, logger, new List<Task>
                {
                        serviceProvider.GetRequiredService<ReportProcess>().ProcessRangeAsync(action, models),
                        serviceProvider.GetRequiredService<CoefficientProcess>().ProcessRangeAsync(action, models)
                });
            }),
            QueueEntities.Float => serviceProvider.GetRequiredService<CoefficientProcess>().ProcessAsync(action, JsonHelper.Deserialize<Float>(data)),
            QueueEntities.Floats => serviceProvider.GetRequiredService<CoefficientProcess>().ProcessRangeAsync(action, JsonHelper.Deserialize<Float[]>(data)),
            QueueEntities.Split => serviceProvider.GetRequiredService<PriceProcess>().ProcessAsync(action, JsonHelper.Deserialize<Split>(data)),
            QueueEntities.Splits => serviceProvider.GetRequiredService<PriceProcess>().ProcessRangeAsync(action, JsonHelper.Deserialize<Split[]>(data)),
            QueueEntities.Coefficient => serviceProvider.GetRequiredService<CoefficientProcess>().ProcessAsync(action, JsonHelper.Deserialize<Coefficient>(data)),
            QueueEntities.Coefficients => serviceProvider.GetRequiredService<CoefficientProcess>().ProcessRangeAsync(action, JsonHelper.Deserialize<Coefficient[]>(data)),
            _ => Task.CompletedTask
        };
    }
}