using AM.Services.Market.Domain.DataAccess;
using AM.Services.Market.Domain.Entities.Interfaces;
using static AM.Services.Market.Enums;

namespace AM.Services.Market.Services.Helpers;

public class StatusChanger<T> where T : class, IRating
{
    private readonly Repository<T> repository;
    public StatusChanger(Repository<T> repository) => this.repository = repository;

    public async Task SetStatusAsync(T model, Statuses status)
    {
        var statusId = (byte)status;
        model.StatusId = statusId;
        await repository.UpdateRangeAsync(new[] { model }, $"Set status '{status}'");
    }
    public async Task SetStatusRangeAsync(T[] models, Statuses status)
    {
        var statusId = (byte)status;

        foreach (var entity in models)
            entity.StatusId = statusId;

        await repository.UpdateRangeAsync(models, $"Set status '{status}' for '{typeof(T).Name}s'");
    }
}