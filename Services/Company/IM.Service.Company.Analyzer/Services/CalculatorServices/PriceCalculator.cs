﻿using CommonServices;
using CommonServices.Models.Dto.CompanyPrices;
using CommonServices.Models.Dto.GatewayCompanies;
using CommonServices.Models.Entity;
using CommonServices.Models.Http;

using IM.Service.Company.Analyzer.Clients;
using IM.Service.Company.Analyzer.DataAccess.Entities;
using IM.Service.Company.Analyzer.DataAccess.Repository;
using IM.Service.Company.Analyzer.Services.CalculatorServices.Interfaces;

using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
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
        private readonly GatewayCompaniesClient gatewayClient;

        public PriceCalculator(
            RepositorySet<Price> repository,
            CompanyPricesClient pricesClient,
            GatewayCompaniesClient gatewayClient)
        {
            this.repository = repository;
            this.pricesClient = pricesClient;
            this.gatewayClient = gatewayClient;
        }

        public async Task<bool> IsSetCalculatingStatusAsync(Price[] prices)
        {
            foreach (var price in prices)
                price.StatusId = (byte)StatusType.Calculating;

            var (errors, _) = await repository.UpdateAsync(prices, $"prices calculating status count: {prices.Length}");

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

            if (!await IsSetCalculatingStatusAsync(prices))
                return false;

            foreach (var priceGroup in prices.GroupBy(x => x.TickerName))
            {
                Price[] result = priceGroup.ToArray();

                var firstElement = priceGroup.OrderBy(x => x.Date).First();
                var targetDate = CommonHelper.GetExchangeLastWorkday(firstElement.SourceType, firstElement.Date);

                try
                {
                    var calculatingData = await GetPricesAsync(priceGroup.Key, targetDate);

                    if (!calculatingData.Any())
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"data for calculating prices by '{priceGroup.Key}' not found!");
                        Console.ForegroundColor = ConsoleColor.Gray;

                        continue;
                    }

                    var calculator = new PriceComparator(calculatingData);
                    result = calculator.GetComparedSample();
                }
                catch (Exception ex)
                {
                    foreach (var price in result)
                        price.StatusId = (byte)StatusType.Error;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"calculating prices for {priceGroup.Key} failed! \nError message: {ex.InnerException?.Message ?? ex.Message}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                await repository.CreateUpdateAsync(result, new PriceComparer(), $"calculate prices for {priceGroup.Key}");
            }

            return true;
        }
        public async Task<bool> CalculateAsync(DateTime dateStart)
        {
            var prices = await repository.GetSampleAsync(x => true);

            if (!prices.Any())
                return false;

            if (!await IsSetCalculatingStatusAsync(prices))
                return false;

            foreach (var priceGroup in prices.GroupBy(x => x.TickerName))
            {
                Price[] result = priceGroup.ToArray();

                try
                {
                    var calculatingData = await GetPricesAsync(priceGroup.Key, dateStart);

                    if (!calculatingData.Any())
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"data for calculating prices by '{priceGroup.Key}' not found!");
                        Console.ForegroundColor = ConsoleColor.Gray;

                        continue;
                    }

                    var calculator = new PriceComparator(calculatingData);
                    result = calculator.GetComparedSample();
                }
                catch (Exception ex)
                {
                    foreach (var price in result)
                        price.StatusId = (byte)StatusType.Error;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"calculating prices for '{priceGroup.Key}' failed! \nError message: {ex.InnerException?.Message ?? ex.Message}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                await repository.CreateUpdateAsync(result, new PriceComparer(), $"calculate prices for {priceGroup.Key}");
            }

            return true;
        }

        private async Task<IReadOnlyCollection<PriceGetDto>> GetPricesAsync(string ticker, DateTime date)
        {
            ResponseModel<PaginatedModel<StockSplitGetDto>> stockSplitsResponse;
            ResponseModel<PaginatedModel<PriceGetDto>> pricesResponse;

            try
            {
                stockSplitsResponse = await gatewayClient.Get<StockSplitGetDto>(
                    "api/stocksplits",
                    GetQueryString(HttpRequestFilterType.More, ticker, date.Year, date.Month, date.Day),
                    new(1, int.MaxValue));

                if (stockSplitsResponse.Errors.Any())
                    throw new BadHttpRequestException(string.Join(';', stockSplitsResponse.Errors));

                pricesResponse = await pricesClient.Get<PriceGetDto>(
                    "prices",
                    GetQueryString(HttpRequestFilterType.More, ticker, date.Year, date.Month, date.Day),
                    new(1, int.MaxValue));

                if (pricesResponse.Errors.Any())
                    throw new BadHttpRequestException(string.Join(';', pricesResponse.Errors));
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"requests for price calculating for '{ticker}' failed! \nError message: {ex.InnerException?.Message ?? ex.Message}");
                Console.ForegroundColor = ConsoleColor.Gray;

                throw;
            }

            //приводим цену в соответствие для рассчета, если по этому тикеру был сплит
            var pricesResponseItems = pricesResponse.Data!.Items;
            var splittedPriceResult = new List<PriceGetDto>(pricesResponse.Data.Count);

            foreach (var stockSplit in stockSplitsResponse.Data!.Items.OrderByDescending(x => x.Date))
            {
                var splittedPrices = pricesResponseItems.Where(x => x.Date >= stockSplit.Date).ToArray();

                foreach (var price in splittedPrices)
                    price.Value *= stockSplit.Divider;

                splittedPriceResult.AddRange(splittedPrices);

                var exceptedResult = pricesResponseItems.Except(splittedPrices, new PriceComparer()).ToArray();

                pricesResponseItems = pricesResponse.Data.Items.Join(exceptedResult, x => (x.TickerName, x.Date),
                    y => (y.TickerName, y.Date), (x, _) => x).ToArray();
            }

            return splittedPriceResult.Union(pricesResponseItems!).ToArray();
        }
    }
}