using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AM.Services.Common.Contracts.Helpers;
using AM.Services.Recommendations.Domain.DataAccess;
using AM.Services.Recommendations.Domain.DataAccess.Comparators;
using AM.Services.Recommendations.Domain.Entities;
using AM.Services.Recommendations.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AM.Services.Recommendations.Services.Entity;

public sealed class SaleService
{
    private const string actionsName = "Sale recommendations";
    private const string actionName = "Sale recommendation";

    private readonly decimal[] percents;
    private readonly ILogger<SaleService> logger;
    private readonly Repository<Sale> saleRepo;
    private readonly Repository<Asset> assetRepo;
    public SaleService(
        ILogger<SaleService> logger,
        IOptionsSnapshot<ServiceSettings> options,
        Repository<Sale> saleRepo,
        Repository<Asset> assetRepo)
    {
        this.logger = logger;
        this.saleRepo = saleRepo;
        this.assetRepo = assetRepo;
        percents = options.Value.SaleSettings.Profits.OrderByDescending(x => x).ToArray();
    }

    public async Task SetAsync(Task<IReadOnlyCollection<Asset>> assetsTask)
    {
        var assets = await assetsTask;
        var ratingCount = await assetRepo.GetCountAsync(x => x.RatingPlace.HasValue);
        var recommendations = new List<Sale>(assets.Count * percents.Length);

        foreach (var asset in assets.Where(x => x.PortfolioBalance is > 0))
        {
            int ratingPlace;

            if (asset.RatingPlace.HasValue)
                ratingPlace = asset.RatingPlace.Value;
            else
            {
                logger.LogWarning(actionsName, "Rating place not found", asset.Name);
                ratingPlace = percents.Length;
                ratingCount = percents.Length;
            }

            var balance = asset.PortfolioBalance!.Value;

            var balanceCost = asset.PortfolioBalanceCost ?? (asset.MarketPriceActual is > 0
                ? balance * asset.MarketPriceActual.Value
                : 0);

            if (balanceCost == 0)
            {
                continue;
            }

            recommendations.AddRange(GetSales(
                asset.Id,
                asset.TypeId,
                balance,
                balanceCost,
                ratingPlace,
                ratingCount,
                asset.MarketPriceActual));
        }

        var assetIds = assets.Select(x => x.Id).Distinct();
        var assetTypeIds = assets.Select(x => x.TypeId).Distinct();

        var sales = await saleRepo.GetSampleAsync(x => assetIds.Contains(x.AssetId) && assetTypeIds.Contains(x.AssetTypeId));
        await saleRepo.DeleteRangeAsync(sales, actionsName);

        if (recommendations.Any())
            await saleRepo.CreateRangeAsync(recommendations, new SaleComparer(), actionsName);
    }
    public async Task SetAsync(IReadOnlyCollection<Asset> assets)
    {
        var ratingCount = await assetRepo.GetCountAsync(x => x.RatingPlace.HasValue);
        var recommendations = new List<Sale>(assets.Count * percents.Length);

        foreach (var asset in assets.Where(x => x.PortfolioBalance is > 0))
        {
            int ratingPlace;

            if (asset.RatingPlace.HasValue)
                ratingPlace = asset.RatingPlace.Value;
            else
            {
                logger.LogWarning(actionsName, "Rating place not found", asset.Name);
                ratingPlace = percents.Length;
                ratingCount = percents.Length;
            }

            var balance = asset.PortfolioBalance!.Value;

            var actualBalanceCost = asset.MarketPriceActual is > 0 ? balance * asset.MarketPriceActual.Value : 0;

            recommendations.AddRange(GetSales(
                asset.Id,
                asset.TypeId,
                balance,
                actualBalanceCost,
                ratingPlace,
                ratingCount,
                asset.MarketPriceActual));
        }

        var assetIds = assets.Select(x => x.Id).Distinct();
        var assetTypeIds = assets.Select(x => x.TypeId).Distinct();

        var salesToDelete = await saleRepo.GetSampleAsync(x => assetIds.Contains(x.AssetId) && assetTypeIds.Contains(x.AssetTypeId));
        await saleRepo.DeleteRangeAsync(salesToDelete, actionsName);

        await saleRepo.CreateRangeAsync(recommendations, new SaleComparer(), actionsName);
    }

    public async Task SetAsync(Asset asset)
    {
        if (!asset.PortfolioBalance.HasValue || !asset.PortfolioBalanceCost.HasValue)
        {
            logger.LogWarning(actionsName, "Deal not found", asset.Name);
            return;
        }

        int ratingCount;
        int ratingPlace;
        if (asset.RatingPlace.HasValue)
        {
            ratingCount = await assetRepo.GetCountAsync(x => x.RatingPlace.HasValue);
            ratingPlace = asset.RatingPlace.Value;
        }
        else
        {
            logger.LogWarning(actionsName, "Rating place not found", asset.Name);
            ratingCount = percents.Length;
            ratingPlace = percents.Length;
        }

        var dealCost = asset.PortfolioBalanceCost!.Value;
        var dealValue = asset.PortfolioBalance!.Value;

        if (dealCost == 0 && asset.MarketPriceActual.HasValue)
        {
            dealValue = Math.Abs(dealValue);
            dealCost = dealValue * asset.MarketPriceActual.Value;
        }

        var recommendations = GetSales(
            asset.Id,
            asset.TypeId,
            dealValue,
            dealCost,
            ratingPlace,
            ratingCount,
            asset.MarketPriceActual)
        .ToArray();

        var salesToDelete = await saleRepo.GetSampleAsync(x => asset.Id == x.AssetId && asset.TypeId == x.AssetTypeId);
        await saleRepo.DeleteRangeAsync(salesToDelete, actionName);

        await saleRepo.CreateRangeAsync(recommendations, new SaleComparer(), actionName);
    }

    public async Task DeleteAsync(IEnumerable<Asset> assets)
    {
        var _assets = assets.ToArray();

        var assetIds = _assets.Select(x => x.Id).Distinct();
        var assetTypeIds = _assets.Select(x => x.TypeId).Distinct();

        var sales = await saleRepo.GetSampleAsync(x => assetIds.Contains(x.AssetId) && assetTypeIds.Contains(x.AssetTypeId));

        await saleRepo.DeleteRangeAsync(sales, actionsName);
    }
    public async Task DeleteAsync(Asset asset)
    {
        var sales = await saleRepo.GetSampleAsync(x => asset.Id == x.AssetId && asset.TypeId == x.AssetTypeId);
        await saleRepo.DeleteRangeAsync(sales, actionsName);
    }

    private IEnumerable<Sale> GetSales(string assetId, byte assetTypeId, decimal balance, decimal balanceCost, int ratingPlace, int ratingCount, decimal? priceActual) =>
        ComputeRecommendations(balance, balanceCost, ratingPlace, ratingCount).Select(y =>
        {
            var profitFact = priceActual / y.PricePlan * 100 - 100;
            return new Sale
            {
                AssetId = assetId,
                AssetTypeId = assetTypeId,
                Balance = y.Balance,
                ProfitPlan = y.ProfitPlan,
                ProfitFact = profitFact,
                PricePlan = y.PricePlan,
                PriceFact = priceActual,
                IsReady = profitFact is > 0
            };
        });
    private IEnumerable<(decimal Balance, decimal ProfitPlan, decimal PricePlan)> ComputeRecommendations(decimal balance, decimal balanceCost, int ratingPlace, int ratingCount)
    {
        var _pricePlan = balanceCost / balance;

        yield return (balance, 0, Math.Round(_pricePlan, 10));

        var activeParts = ComputeActiveParts(ratingPlace, ratingCount);

        foreach (var profitPercent in percents)
        {
            if (balance <= 0 || !activeParts.ContainsKey(profitPercent))
                yield break;

            var _balance = Math.Round(balance * activeParts[profitPercent] / 100);

            if (_balance == 0)
                _balance = balance;

            balance -= _balance;

            var pricePlan = _pricePlan + _pricePlan * profitPercent / 100;
            yield return (_balance, profitPercent, Math.Round(pricePlan, 10));
        }
    }
    private Dictionary<decimal, decimal> ComputeActiveParts(int ratingPlace, int ratingCount)
    {
        var partsCount = percents.Length;
        var ratingPercent = 100 - (decimal)ratingPlace * 100 / ratingCount;
        decimal resultPercent = 100;

        var result = new Dictionary<decimal, decimal>(percents.Length);
        foreach (var profitPercent in percents)
        {
            if (resultPercent <= 0)
                break;

            if (partsCount == 1)
            {
                result.Add(profitPercent, resultPercent);
                break;
            }

            var _resultPercent = resultPercent * ratingPercent / 100;

            result.Add(profitPercent, _resultPercent);

            resultPercent -= _resultPercent;
            partsCount--;
        }

        return result;
    }
}