﻿using IM.Service.Common.Net;
using IM.Service.MarketData.Clients;
using IM.Service.MarketData.Domain.DataAccess;
using IM.Service.MarketData.Domain.DataAccess.Comparators;
using IM.Service.MarketData.Domain.Entities;
using IM.Service.MarketData.Domain.Entities.ManyToMany;
using IM.Service.MarketData.Models.Clients;

namespace IM.Service.MarketData.Services.DataLoaders.Prices.Implementations;

public class TdameritradeGrabber : IDataGrabber
{
    private readonly Repository<Price> repository;
    private readonly ILogger<DataLoader<Price>> logger;
    private readonly TdAmeritradeClient client;

    public TdameritradeGrabber(Repository<Price> repository, ILogger<DataLoader<Price>> logger, TdAmeritradeClient client)
    {
        this.repository = repository;
        this.logger = logger;
        this.client = client;
    }

    public async Task GetCurrentDataAsync(CompanySource companySource)
    {
        try
        {
            if (companySource.Value is null)
                throw new ArgumentNullException(companySource.CompanyId);

            var data = await client.GetCurrentPricesAsync(new[] { companySource.CompanyId });

            var result = Map(data);

            result = result.Where(x => x.Date == DateOnly.FromDateTime(DateTime.UtcNow)).ToArray();

            await repository.CreateUpdateAsync(result, new DataDateComparer<Price>(), "Tdameritrade current prices");
        }
        catch (Exception exception)
        {
            logger.LogError(LogEvents.Processing, "Place: {place}. Error: {exception}", nameof(TdameritradeGrabber) + '.' + nameof(GetCurrentDataAsync), exception.InnerException?.Message ?? exception.Message);
        }
    }
    public async Task GetCurrentDataAsync(IEnumerable<CompanySource> companySources)
    {
        try
        {
            var data = await client.GetCurrentPricesAsync(companySources.Select(x => x.CompanyId));

            var result = Map(data);

            result = result.Where(x => x.Date == DateOnly.FromDateTime(DateTime.UtcNow)).ToArray();

            await repository.CreateUpdateAsync(result, new DataDateComparer<Price>(), "Tdameritrade current prices");
        }
        catch (Exception exception)
        {
            logger.LogError(LogEvents.Processing, "Place: {place}. Error: {exception}", nameof(TdameritradeGrabber) + '.' + nameof(GetCurrentDataAsync), exception.InnerException?.Message ?? exception.Message);
        }
    }

    public async Task GetHistoryDataAsync(CompanySource companySource)
    {
        try
        {
            if (companySource.Value is null)
                throw new ArgumentNullException(companySource.CompanyId);

            var data = await client.GetHistoryPricesAsync(companySource.CompanyId);

            var result = Map(data);

            await repository.CreateAsync(result, new DataDateComparer<Price>(), "Tdameritrade history prices");
        }
        catch (Exception exception)
        {
            logger.LogError(LogEvents.Processing, "Place: {place}. Error: {exception}", nameof(TdameritradeGrabber) + '.' + nameof(GetHistoryDataAsync), exception.InnerException?.Message ?? exception.Message);
        }
    }
    public async Task GetHistoryDataAsync(IEnumerable<CompanySource> companySources)
    {
        using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(100));

        foreach (var config in companySources)
            if (await timer.WaitForNextTickAsync())
                await GetHistoryDataAsync(config);
    }

    private static Price[] Map(TdAmeritradeLastPriceResultModel clientResult) =>
        clientResult.data is null
            ? Array.Empty<Price>()
            : clientResult.data.Select(x => new Price
            {
                Date = DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeMilliseconds(x.Value.regularMarketTradeTimeInLong).DateTime),
                Value = x.Value.lastPrice,
                CompanyId = x.Key,
                SourceId = (byte)Enums.Sources.Tdameritrade
            }).ToArray();
    private static Price[] Map(TdAmeritradeHistoryPriceResultModel clientResult) =>
        clientResult.data?.candles is null
            ? Array.Empty<Price>()
            : clientResult.data.candles.Select(x => new Price
            {
                Date = DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeMilliseconds(x.datetime).DateTime),
                Value = x.high,
                CompanyId = clientResult.ticker,
                SourceId = (byte)Enums.Sources.Tdameritrade
            }).ToArray();
}