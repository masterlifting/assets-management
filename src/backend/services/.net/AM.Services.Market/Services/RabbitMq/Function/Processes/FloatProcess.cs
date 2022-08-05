using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Market.Domain.Entities.ManyToMany;
using AM.Services.Market.Services.Entity;

namespace AM.Services.Market.Services.RabbitMq.Function.Processes;

public sealed class FloatProcess : IRabbitProcess
{
    private readonly FloatService service;
    public FloatProcess(FloatService service) => this.service = service;

    public Task ProcessAsync<T>(QueueActions action, T model) where T : class => action switch
    {
        QueueActions.Get => model switch
        {
            CompanySource companySource => service.Loader.LoadAsync(companySource),
            _ => Task.CompletedTask
        },
        _ => Task.CompletedTask
    };
    public Task ProcessRangeAsync<T>(QueueActions action, IEnumerable<T> models) where T : class => action switch
    {
        QueueActions.Get => models switch
        {
            CompanySource[] companySources => service.Loader.LoadAsync(companySources),
            _ => Task.CompletedTask
        },
        _ => Task.CompletedTask
    };
}