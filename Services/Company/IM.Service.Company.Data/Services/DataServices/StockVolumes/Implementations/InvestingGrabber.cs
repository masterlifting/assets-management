using HtmlAgilityPack;

using IM.Service.Common.Net;
using IM.Service.Common.Net.RepositoryService.Comparators;
using IM.Service.Company.Data.Clients.Report;
using IM.Service.Company.Data.DataAccess.Entities;
using IM.Service.Company.Data.DataAccess.Repository;
using IM.Service.Company.Data.Models.Data;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IM.Service.Company.Data.Services.DataServices.StockVolumes.Implementations;

public class InvestingGrabber : IDataGrabber
{
    private readonly Repository<StockVolume> repository;
    private readonly ILogger<StockVolumeGrabber> logger;
    private readonly InvestingParserHandler handler;

    public InvestingGrabber(Repository<StockVolume> repository, ILogger<StockVolumeGrabber> logger, InvestingClient client)
    {
        this.repository = repository;
        this.logger = logger;
        handler = new(client);
    }

    public async Task GrabCurrentDataAsync(string source, DataConfigModel config) => await GrabHistoryDataAsync(source, config);
    public async Task GrabCurrentDataAsync(string source, IEnumerable<DataConfigModel> configs) => await GrabHistoryDataAsync(source, configs);

    public async Task GrabHistoryDataAsync(string source, DataConfigModel config)
    {
        try
        {
            if (config.SourceValue is null)
                throw new ArgumentNullException(config.CompanyId);

            var site = await handler.LoadDataAsync(config.SourceValue);
            var result = InvestingParserHandler.Parse(site, config.CompanyId, source);

            var (error, _) = await repository.CreateUpdateAsync(result, new CompanyDateComparer<StockVolume>(), "Investing history stock volumes");

            if (error is not null)
                throw new Exception("Repository exception!");
        }
        catch (Exception exception)
        {
            logger.LogError(LogEvents.Processing, "Place: {place}. Error: {exception}", nameof(InvestingGrabber) + '.' + nameof(GrabCurrentDataAsync), exception.InnerException?.Message ?? exception.Message);
        }
    }
    public async Task GrabHistoryDataAsync(string source, IEnumerable<DataConfigModel> configs)
    {
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(10));

        foreach (var config in configs)
            if (await timer.WaitForNextTickAsync())
                await GrabHistoryDataAsync(source, config);
    }
}

internal class InvestingParserHandler
{
    private readonly InvestingClient client;
    internal InvestingParserHandler(InvestingClient client) => this.client = client;

    internal async Task<HtmlDocument> LoadDataAsync(string sourceValue) => await client.GetMainPageAsync(sourceValue);

    internal static IEnumerable<StockVolume> Parse(HtmlDocument site, string companyId, string sourceName)
    {
        var result = new StockVolume[4];
        var mainPage = new MainPage(site);

        for (var i = 0; i < result.Length; i++)
            result[i] = new()
            {
                CompanyId = companyId,
                SourceType = sourceName,
                Date = DateTime.UtcNow,
                Value = mainPage.StockVolume
            };

        return result;
    }

    private class MainPage
    {
        private readonly HtmlDocument page;
        public MainPage(HtmlDocument? page)
        {
            this.page = page ?? throw new NullReferenceException("Loaded page is null");
            StockVolume = GetStockVolume();
        }

        public long StockVolume { get; }

        private long GetStockVolume()
        {
            var stockVolumeData = page.DocumentNode.SelectNodes("//dt").FirstOrDefault(x => x.InnerText == "Акции в обращении")?.NextSibling?.InnerText;
            return stockVolumeData is not null
                   && long.TryParse(
                       stockVolumeData.Replace(".", "")
                       , NumberStyles.AllowCurrencySymbol, new CultureInfo("Ru-ru"), out var result)
                ? result
                : throw new NotSupportedException("Stock volume parsing is failed");
        }
    }
}