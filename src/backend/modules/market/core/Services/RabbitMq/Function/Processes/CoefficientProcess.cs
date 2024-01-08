using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Market.Domain.Entities;
using AM.Services.Market.Services.Entity;
using static AM.Services.Market.Enums;

namespace AM.Services.Market.Services.RabbitMq.Function.Processes;

public sealed class CoefficientProcess : IRabbitProcess
{
    private readonly CoefficientService service;
    public CoefficientProcess(CoefficientService service) => this.service = service;

    public Task ProcessAsync<T>(QueueActions action, T model) where T : class => action switch
    {
        QueueActions.Set => model switch
        {
            Coefficient coefficient => service.SetStatusAsync(coefficient, Statuses.Ready),
            _ => Task.CompletedTask
        },
        QueueActions.Create or QueueActions.Update or QueueActions.Delete => model switch
        {
            Report report => service.SetCoefficientAsync(action, report),
            Price price => service.SetCoefficientAsync(action, price),
            Float _float => service.SetCoefficientAsync(action, _float),
            _ => Task.CompletedTask
        },
        _ => Task.CompletedTask
    };
    public Task ProcessRangeAsync<T>(QueueActions action, IEnumerable<T> models) where T : class => action switch
    {
        QueueActions.Set => models switch
        {
            Coefficient[] coefficients => service.SetStatusRangeAsync(coefficients, Statuses.Ready),
            _ => Task.CompletedTask
        },
        QueueActions.Create or QueueActions.Update or QueueActions.Delete => models switch
        {
            Report[] reports => service.SetCoefficientAsync(action, reports),
            Price[] prices => service.SetCoefficientAsync(action, prices),
            Float[] floats => service.SetCoefficientAsync(action, floats),
            _ => Task.CompletedTask
        },
        _ => Task.CompletedTask
    };
}