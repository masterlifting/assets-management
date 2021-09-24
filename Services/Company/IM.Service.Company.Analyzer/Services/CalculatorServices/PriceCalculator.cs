﻿using CommonServices;
using CommonServices.Models.Dto.CompanyPrices;
using CommonServices.Models.Dto.GatewayCompanies;
using CommonServices.Models.Entity;

using IM.Service.Company.Analyzer.Clients;
using IM.Service.Company.Analyzer.DataAccess.Entities;
using IM.Service.Company.Analyzer.DataAccess.Repository;
using IM.Service.Company.Analyzer.Services.CalculatorServices.Interfaces;

using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using static CommonServices.CommonEnums;
using static CommonServices.HttpServices.QueryStringBuilder;
using static IM.Service.Company.Analyzer.Enums;

namespace IM.Service.Company.Analyzer.Services.CalculatorServices
{
    public class PriceCalculator : IAnalyzerCalculator<Price>
    {
        private readonly RepositorySet<Price> repository;
        private readonly CompanyPricesClient pricesClient;
        private readonly CompaniesClient gatewayClient;

        public PriceCalculator(
            RepositorySet<Price> repository,
            CompanyPricesClient pricesClient,
            CompaniesClient gatewayClient)
        {
            this.repository = repository;
            this.pricesClient = pricesClient;
            this.gatewayClient = gatewayClient;
        }

        public async Task<bool> IsSetCalculatingStatusAsync(Price[] prices, string info)
        {
            foreach (var price in prices)
                price.StatusId = (byte)StatusType.Calculating;

            var (errors, _) = await repository.UpdateAsync(prices, $"price calculating status for '{info}'");

            return !errors.Any();
        }

        public async Task<bool> CalculateAsync()
        {
            var prices = await repository.GetSampleAsync(x =>
                x.StatusId == (byte)StatusType.ToCalculate
                || x.StatusId == (byte)StatusType.CalculatedPartial
                || x.StatusId == (byte)StatusType.Error);

            if (!prices.Any())
                return false;

            foreach (var group in prices.GroupBy(x => x.TickerName))
                await BaseCalculateAsync(group);

            return true;
        }
        public async Task<bool> CalculateAsync(DateTime dateStart)
        {
            var prices = await repository.GetSampleAsync(x => x.Date >= dateStart);

            if (!prices.Any())
                return false;

            foreach (var group in prices.GroupBy(x => x.TickerName))
                await BaseCalculateAsync(group);

            return true;
        }

        private async Task BaseCalculateAsync(IGrouping<string, Price> group)
        {
            var orderedPrices = group.OrderBy(x => x.Date).ToArray();

            var dateTarget = orderedPrices[0].Date.AddDays(-10);
            var dateStart = CommonHelper.GetExchangeLastWorkday(orderedPrices[0].SourceType, dateTarget);
            var ticker = group.Key;

            if (!await IsSetCalculatingStatusAsync(orderedPrices, ticker))
                throw new DataException($"set price calculating status for '{ticker}' failed");

            try
            {
                var calculatingData = await GetCalculatingDataAsync(ticker, dateStart);

                if (!calculatingData.Any())
                    throw new DataException($"prices for '{ticker}' not found!");

                var calculator = new PriceComparator(calculatingData);
                var calculateResult = calculator.GetComparedSample();

                await repository.CreateUpdateAsync(calculateResult, new PriceComparer(), $"price calculated result for {ticker}");
            }
            catch (Exception ex)
            {
                foreach (var price in orderedPrices)
                    price.StatusId = (byte)StatusType.Error;

                await repository.CreateUpdateAsync(orderedPrices, new PriceComparer(), $"calculate prices for {ticker}");

                throw new ArithmeticException($"price calculated for '{ticker}' failed! \nError: {ex.Message}");
            }
        }
        private async Task<IReadOnlyCollection<PriceGetDto>> GetCalculatingDataAsync(string ticker, DateTime date)
        {
            var stockSplitsResponse = await gatewayClient.Get<StockSplitGetDto>(
                "stocksplits",
                GetQueryString(HttpRequestFilterType.More, ticker, date.Year, date.Month, date.Day),
                new(1, int.MaxValue));

            if (stockSplitsResponse.Errors.Any())
                throw new BadHttpRequestException(string.Join('\n', stockSplitsResponse.Errors));

            var pricesResponse = await pricesClient.Get<PriceGetDto>(
                "prices",
                GetQueryString(HttpRequestFilterType.More, ticker, date.Year, date.Month, date.Day),
                new(1, int.MaxValue));

            return pricesResponse.Errors.Any()
                ? throw new BadHttpRequestException(string.Join('\n', pricesResponse.Errors))
                : SeparateSplittedPrices(pricesResponse.Data!.Items, stockSplitsResponse.Data!.Items);
        }
        //приводим цены в соответствие для рассчета, если по этому тикеру был сплит
        private static PriceGetDto[] SeparateSplittedPrices(PriceGetDto[] priceData, IEnumerable<StockSplitGetDto> stockSplitData)
        {
            var pricesResponseItems = priceData;
            var splittedPriceResult = new List<PriceGetDto>(priceData.Length);

            foreach (var stockSplit in stockSplitData.OrderByDescending(x => x.Date))
            {
                var splittedPrices = pricesResponseItems.Where(x => x.Date >= stockSplit.Date).ToArray();

                foreach (var price in splittedPrices)
                    price.Value /= stockSplit.Divider;

                splittedPriceResult.AddRange(splittedPrices);

                var exceptedResult = pricesResponseItems.Except(splittedPrices, new PriceComparer()).ToArray();

                pricesResponseItems = priceData.Join(exceptedResult, x => (x.TickerName, x.Date),
                    y => (y.TickerName, y.Date), (x, _) => x).ToArray();
            }

            return splittedPriceResult.Union(pricesResponseItems).ToArray();
        }
    }
}