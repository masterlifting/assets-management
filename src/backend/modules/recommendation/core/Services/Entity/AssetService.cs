using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AM.Services.Common.Contracts.Models.RabbitMq.Api;
using AM.Services.Recommendations.Domain.DataAccess;
using AM.Services.Recommendations.Domain.Entities;

namespace AM.Services.Recommendations.Services.Entity;

public sealed class AssetService
{
    private const string logPrefix = $"{nameof(AssetService)}";
    private const string actionName = "Set asset";
    private const string actionsName = "Set assets";

    private readonly Repository<Asset> assetRepo;
    public AssetService(Repository<Asset> assetRepo) => this.assetRepo = assetRepo;

    public async Task<Asset> SetAsync(AssetMarketMqDto dto)
    {
        var (assetId, assetTypeId, priceLast, priceAvg) = dto;
        var asset = await assetRepo.FindAsync(assetId, assetTypeId );
        if (asset is not null)
        {
            asset.MarketPriceActual = priceLast;
            asset.MarketPriceAvg = priceAvg;

            await assetRepo.UpdateAsync(new object[] { asset.Id, assetTypeId }, asset, actionName + ':' + assetId);
            return asset;
        }
        throw new DataException($"{logPrefix}.{actionName}.Error: {assetId} not found");
    }
    public async Task<Asset> SetAsync(AssetPortfolioMqDto dto)
    {
        var (assetId, assetTypeId, sumValue, sumCost, dealPrice) = dto;
        var asset = await assetRepo.FindAsync( assetId, assetTypeId );
        if (asset is not null)
        {
            asset.PortfolioBalanceCost = sumCost;
            asset.PortfolioBalance = sumValue;
            asset.PortfolioLastDealPrice = dealPrice;
            await assetRepo.UpdateAsync(new object[] { asset.Id, assetTypeId }, asset, actionName + ':' + assetId);
            return asset;
        }
        throw new DataException($"{logPrefix}.{actionName}.Error: {assetId} not found");
    }
    public async Task<Asset[]> SetAsync(IReadOnlyCollection<AssetMarketMqDto> dtos)
    {
        var groupedData = dtos
            .GroupBy(x => (x.AssetId, x.AssetTypeId))
            .ToDictionary(x => x.Key, y => y.First());

        var assetIds = groupedData.Select(x => x.Key.AssetId).Distinct();
        var assetTypeIds = groupedData.Select(x => x.Key.AssetTypeId).Distinct();
        var dbAssets = await assetRepo.GetSampleAsync(x => assetIds.Contains(x.Id) && assetTypeIds.Contains(x.TypeId));

        var assets = dbAssets.Join(groupedData, x => (x.Id, x.TypeId), y => y.Key, (x, y) =>
        {
            var (_, (_, _, costFact, costAvg)) = y;
            x.MarketPriceActual = costFact;
            x.MarketPriceAvg = costAvg;
            return x;
        })
        .ToArray();

        await assetRepo.UpdateRangeAsync(assets, actionsName);
        return assets;
    }
    public async Task<IReadOnlyCollection<Asset>> SetAsync(IReadOnlyCollection<AssetPortfolioMqDto> dtos)
    {
        var groupedData = dtos
            .GroupBy(x => (x.AssetId, x.AssetTypeId))
            .ToDictionary(x => x.Key, y => y.First());

        var assetIds = groupedData.Select(x => x.Key.AssetId).Distinct();
        var assetTypeIds = groupedData.Select(x => x.Key.AssetTypeId).Distinct();
        var dbAssets = await assetRepo.GetSampleAsync(x => assetIds.Contains(x.Id) && assetTypeIds.Contains(x.TypeId));

        var assets = dbAssets.Join(groupedData, x => (x.Id, x.TypeId), y => y.Key, (x, y) =>
            {
                var (_, (_, _, balance, balanceCost, lastDealPrice)) = y;
                x.PortfolioBalance = balance;
                x.PortfolioBalanceCost = balanceCost;
                x.PortfolioLastDealPrice = lastDealPrice;
                return x;
            })
            .ToArray();

        await assetRepo.UpdateRangeAsync(assets, actionsName);
        return assets;
    }
    public async Task<Asset[]> SetAsync(IEnumerable<AssetRatingMqDto> dtos)
    {
        var groupedData = dtos
            .GroupBy(x => (x.AssetId, x.AssetTypeId))
            .ToDictionary(x => x.Key, y => y.First());

        var assetIds = groupedData.Select(x => x.Key.AssetId).Distinct();
        var assetTypeIds = groupedData.Select(x => x.Key.AssetTypeId).Distinct();
        var dbAssets = await assetRepo.GetSampleAsync(x => assetIds.Contains(x.Id) && assetTypeIds.Contains(x.TypeId));

        var assets = dbAssets.Join(groupedData, x => (x.Id, x.TypeId), y => y.Key, (x, y) =>
        {
            x.RatingPlace = y.Value.Place;
            return x;
        })
        .ToArray();

        await assetRepo.UpdateRangeAsync(assets, actionsName);
        return assets;
    }
}