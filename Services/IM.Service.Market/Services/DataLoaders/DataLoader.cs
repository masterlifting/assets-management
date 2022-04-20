﻿using IM.Service.Common.Net;
using IM.Service.Common.Net.Helpers;
using IM.Service.Common.Net.Models.Entity.Interfaces;
using IM.Service.Market.Domain.DataAccess;
using IM.Service.Market.Domain.Entities;
using IM.Service.Market.Domain.Entities.Interfaces;
using IM.Service.Market.Domain.Entities.ManyToMany;

using System.Runtime.Serialization;

namespace IM.Service.Market.Services.DataLoaders;

public class DataLoader<TEntity> where TEntity : class, IDataIdentity, IPeriod
{
    private readonly ILogger<DataLoader<TEntity>> logger;
    private readonly ILastDataHelper<TEntity> lastDataHelper;

    private readonly DataGrabber grabber;

    protected Func<TEntity, bool> IsCurrentDataCondition { get; init; }
    protected int TimeAgo { get; init; }

    protected DataLoader(ILogger<DataLoader<TEntity>> logger, Repository<TEntity> repository, Dictionary<byte, IDataGrabber> sourceMatcher)
    {
        this.logger = logger;

        grabber = new DataGrabber(sourceMatcher);

        IsCurrentDataCondition = _ => true;
        TimeAgo = 1;

        lastDataHelper = repository switch
        {
            Repository<Price> repo => (new LastDateHelper<Price>(repo, TimeAgo) as ILastDataHelper<TEntity>)!,
            Repository<Split> repo => (new LastDateHelper<Split>(repo, TimeAgo) as ILastDataHelper<TEntity>)!,
            Repository<Float> repo => (new LastDateHelper<Float>(repo, TimeAgo) as ILastDataHelper<TEntity>)!,
            Repository<Dividend> repo => (new LastDateHelper<Dividend>(repo, TimeAgo) as ILastDataHelper<TEntity>)!,
            Repository<Report> repo => (new LastQuarterHelper<Report>(repo, TimeAgo) as ILastDataHelper<TEntity>)!,
            _ => throw new ArgumentOutOfRangeException(nameof(repository), repository, null)
        };
    }

    public Task LoadDataAsync(string data) =>
        JsonHelper.TryDeserialize(data, out CompanySource? entity)
            ? DataSetAsync(entity!)
            : throw new SerializationException(typeof(TEntity).Name);
    public Task LoadDataRangeAsync(string data) =>
        JsonHelper.TryDeserialize(data, out CompanySource[]? entities)
            ? DataSetAsync(entities!)
            : throw new SerializationException(typeof(TEntity).Name);


    private async Task DataSetAsync(CompanySource companySource)
    {
        if (!grabber.ToContinue(companySource))
            return;

        var last = await lastDataHelper.GetLastDataAsync(companySource);

        var isCurrent = last is not null && IsCurrentDataCondition.Invoke(last);

        await grabber.GetCurrentDataAsync(companySource);

        if (!isCurrent)
            await grabber.GetHistoryDataAsync(companySource);

        logger.LogInformation(LogEvents.Processing, "Grab '{data}' for '{name}' by '{value}' is complete. ", typeof(TEntity).Name, companySource.CompanyId, companySource.Value);
    }
    private async Task DataSetAsync(CompanySource[] companySources)
    {
        if (!grabber.ToContinue(companySources))
            return;

        var lasts = await lastDataHelper.GetLastDataAsync(companySources);

        if (!lasts.Any())
        {
            await grabber.GetHistoryDataAsync(companySources);
        }
        else
        {
            var currentData = lasts.Where(x => IsCurrentDataCondition.Invoke(x));
            var historyData = lasts.Where(x => !IsCurrentDataCondition.Invoke(x));

            await grabber.GetCurrentDataAsync(companySources.Join(currentData, x => (x.CompanyId, x.SourceId), y => (y.CompanyId, y.SourceId), (x, _) => x));
            await grabber.GetHistoryDataAsync(companySources.Join(historyData, x => (x.CompanyId, x.SourceId), y => (y.CompanyId, y.SourceId), (x, _) => x));
        }

        logger.LogInformation(LogEvents.Processing, "Grab '{data}' for '{name}' is complete. ", typeof(TEntity).Name, string.Join(",", companySources.Select(x => x.CompanyId).Distinct()));
    }
}

public interface ILastDataHelper<TEntity> where TEntity : class, IDataIdentity, IPeriod
{
    Task<TEntity?> GetLastDataAsync(CompanySource companySource);
    Task<TEntity[]> GetLastDataAsync(CompanySource[] companySources);
}
public sealed class LastQuarterHelper<TEntity> : ILastDataHelper<TEntity> where TEntity : class, IDataIdentity, IQuarterIdentity
{
    private readonly Repository<TEntity> repository;
    private readonly int yearsAgo;

    public LastQuarterHelper(Repository<TEntity> repository, int yearsAgo)
    {
        this.repository = repository;
        this.yearsAgo = yearsAgo;
    }

    public async Task<TEntity?> GetLastDataAsync(CompanySource companySource)
    {
        var result = await repository.GetSampleAsync(x =>
                x.CompanyId == companySource.CompanyId
                && x.SourceId == companySource.SourceId
                && x.Year >= DateTime.UtcNow.AddYears(-yearsAgo).Year);

        return result
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Quarter)
            .LastOrDefault();
    }
    public async Task<TEntity[]> GetLastDataAsync(CompanySource[] companySources)
    {
        var companyIds = companySources.Select(x => x.CompanyId).Distinct();
        var sourceIds = companySources.Select(x => x.SourceId).Distinct();

        var result = await repository.GetSampleAsync(x =>
            companyIds.Contains(x.CompanyId)
            && sourceIds.Contains(x.SourceId)
            && x.Year >= DateTime.UtcNow.AddYears(-yearsAgo).Year);

        return result
            .GroupBy(x => (x.CompanyId, x.SourceId))
            .Select(x =>
                x.OrderBy(y => y.Year)
                    .ThenBy(y => y.Quarter)
                    .Last())
            .ToArray();
    }
}
public sealed class LastDateHelper<TEntity> : ILastDataHelper<TEntity> where TEntity : class, IDataIdentity, IDateIdentity
{
    private readonly Repository<TEntity> repository;
    private readonly int daysAgo;

    public LastDateHelper(Repository<TEntity> repository, int daysAgo)
    {
        this.repository = repository;
        this.daysAgo = daysAgo;
    }

    public async Task<TEntity?> GetLastDataAsync(CompanySource companySource)
    {
        var result = await repository.GetSampleOrderedAsync(x =>
            x.CompanyId == companySource.CompanyId
            && x.SourceId == companySource.SourceId
            && x.Date >= DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-daysAgo),
            orderBy => orderBy.Date);

        return result.LastOrDefault();
    }
    public async Task<TEntity[]> GetLastDataAsync(CompanySource[] companySources)
    {
        var companyIds = companySources.Select(x => x.CompanyId).Distinct();
        var sourceIds = companySources.Select(x => x.SourceId).Distinct();

        var result = await repository.GetSampleAsync(x =>
            companyIds.Contains(x.CompanyId)
            && sourceIds.Contains(x.SourceId)
            && x.Date >= DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-daysAgo));

        return result
            .GroupBy(x => (x.CompanyId, x.SourceId))
            .Select(x => x.OrderBy(y => y.Date).Last())
            .ToArray();
    }
}