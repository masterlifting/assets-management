﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AM.Services.Common.Contracts.Models.Service;
using AM.Services.Recommendations.Domain.DataAccess;
using AM.Services.Recommendations.Domain.Entities;
using AM.Services.Recommendations.Domain.Entities.Catalogs;
using AM.Services.Recommendations.Models.Api.Http;
using Microsoft.EntityFrameworkCore;
using static AM.Services.Common.Contracts.Helpers.ServiceHelper;

namespace AM.Services.Recommendations.Services.Http;

public class SaleApi
{
    private readonly Repository<Asset> assetRepo;
    private readonly Repository<AssetType> assetTypeRepo;
    private readonly Repository<Sale> saleRepo;
    public SaleApi(Repository<Asset> assetRepo, Repository<AssetType> assetTypeRepo, Repository<Sale> saleRepo)
    {
        this.assetRepo = assetRepo;
        this.assetTypeRepo = assetTypeRepo;
        this.saleRepo = saleRepo;
    }

    public async Task<PaginationModel<SaleDto>> GetAsync(Paginatior pagination, Expression<Func<Sale, bool>> filter)
    {
        var queryFilter = saleRepo.Where(filter);
        var count = await saleRepo.GetCountAsync(queryFilter);
        var paginatedResult = saleRepo.GetPaginationQueryDesc(queryFilter, pagination, x => x.ProfitFact);

        var sales = await paginatedResult
            .OrderByDescending(x => x.ProfitFact)
            .Select(x => new
            {
                x.AssetId,
                x.Asset.Name,
                x.AssetTypeId,
                x.Balance,
                x.ProfitPlan,
                x.ProfitFact,
                x.PricePlan,
                x.PriceFact
            })
            .ToArrayAsync();

        var result = sales
            .GroupBy(x => (x.AssetId, x.AssetTypeId))
            .Select(x => new SaleDto
            {
                Asset = $"({x.Key.AssetId}) " + x.First().Name,
                Recommendations = x
                    .OrderBy(y => y.ProfitPlan)
                    .Select(y => new SaleRecommendationDto(y.ProfitPlan, y.ProfitFact, y.Balance, y.PricePlan, y.PriceFact))
                .ToArray()
            }).ToArray();

        return new PaginationModel<SaleDto>
        {
            Items = result,
            Count = count
        };
    }
    public async Task<SaleDto> GetAsync(byte assetTypeId, string assetId)
    {
        assetId = assetId.ToUpperInvariant();

        var asset = await assetRepo.FindAsync(assetId, assetTypeId);

        if (asset is null)
            throw new NullReferenceException($"'{assetId}' not found");

        var sales = await saleRepo.GetSampleAsync(x => x.AssetId == asset.Id && x.AssetTypeId == asset.TypeId);

        return new SaleDto
        {
            Asset = $"({asset.Id}) " + asset.Name,
            Recommendations = sales
                .OrderBy(x => x.ProfitPlan)
                .Select(x => new SaleRecommendationDto(x.ProfitPlan, x.ProfitFact, x.Balance, x.PricePlan, x.PriceFact))
                .ToArray()
        };
    }
    public async Task<AssetTypeDto[]> GetAssetsAsync()
    {
        var entities = await assetTypeRepo.GetSampleAsync(x => x);
        return entities.Select(x => new AssetTypeDto(x.Id, x.Name, x.Description)).ToArray();
    }
}