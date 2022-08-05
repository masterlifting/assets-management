using System.Collections.Generic;
using System.Threading.Tasks;
using AM.Services.Common.Contracts.Helpers;
using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Portfolio.Models.Api.Mq;
using AM.Services.Portfolio.Services.Entity;

namespace AM.Services.Portfolio.Services.RabbitMq.Function.Processes;

public sealed class ReportProcess : IRabbitProcess
{
    private readonly ReportService service;
    public ReportProcess(ReportService service) => this.service = service;

    public Task ProcessAsync<T>(QueueActions action, T model) where T : class => action switch
    {
        QueueActions.Get => model switch
        {
            ProviderReportDto report => service.SetAsync(report),
            _ => service.Logger.LogDefaultTask($"{action} {typeof(T).Name}"),
        },
        _ => service.Logger.LogDefaultTask($"{action}")
    };
    public Task ProcessRangeAsync<T>(QueueActions action, IEnumerable<T> models) where T : class
        => service.Logger.LogDefaultTask($"{action} {typeof(T).Name}");
}