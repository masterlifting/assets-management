using AM.Services.Common.Contracts.Helpers;
using AM.Services.Common.Contracts.Models.Entity.Interfaces;
using AM.Services.Market.Domain.DataAccess;
using AM.Services.Market.Domain.Entities.Interfaces;
using AM.Services.Market.Domain.Entities.ManyToMany;

namespace AM.Services.Market.Services.Data;

public sealed class DataLoader<TEntity> where TEntity : class, IDataIdentity, IPeriod
{
    private readonly ILogger<DataLoader<TEntity>> logger;
    private readonly Repository<TEntity> repository;
    private readonly IDataLoaderConfiguration<TEntity> configuration;
    private readonly string tName;

    public DataLoader(ILogger<DataLoader<TEntity>> logger, Repository<TEntity> repository, IDataLoaderConfiguration<TEntity> configuration)
    {
        this.logger = logger;
        this.repository = repository;
        this.configuration = configuration;
        tName = typeof(TEntity).Name;
    }

    public async Task LoadAsync(CompanySource companySource)
    {
        if (!configuration.Grabber.ToContinue(companySource))
            return;

        await foreach (var data in configuration.Grabber.GetCurrentDataAsync(companySource))
            await repository.CreateUpdateRangeAsync(data, configuration.Comparer, $"Current '{tName}s' saved");

        await foreach (var data in configuration.Grabber.GetHistoryDataAsync(companySource))
            await repository.CreateUpdateRangeAsync(data, configuration.Comparer, $"History '{tName}s' saved");

        logger.LogDebug("Data loading", tName, "Done");
    }
    public async Task LoadAsync(CompanySource[] companySources)
    {
        if (!configuration.Grabber.ToContinue(companySources))
            return;

        var lasts = await configuration.LastDataHelper.GetLastDataAsync(companySources);

        if (!lasts.Any())
            await foreach (var data in configuration.Grabber.GetHistoryDataAsync(companySources))
                await repository.CreateRangeAsync(data, configuration.Comparer, $"History '{tName}s' saved");
        else
        {
            var currentData = lasts.Where(x => configuration.IsCurrentDataCondition.Invoke(x)).ToArray();
            var historyData = lasts.Where(x => !configuration.IsCurrentDataCondition.Invoke(x)).ToArray();

            await foreach (var data in configuration.Grabber.GetCurrentDataAsync(companySources.Join(currentData, x => (x.CompanyId, x.SourceId), y => (y.CompanyId, y.SourceId), (x, _) => x)))
                await repository.CreateUpdateRangeAsync(data, configuration.Comparer, $"Current '{tName}s' saved");

            await foreach (var data in configuration.Grabber.GetHistoryDataAsync(companySources.Join(historyData, x => (x.CompanyId, x.SourceId), y => (y.CompanyId, y.SourceId), (x, _) => x)))
                await repository.CreateRangeAsync(data, configuration.Comparer, $"History '{tName}s' saved");
        }

        logger.LogDebug("Data loading", tName, "Done");
    }
}