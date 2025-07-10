using AM.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Bcs.Companies;
using AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs.Companies;
using AM.Portfolio.Core.Persistence.Entities.Sql;

using static AM.Portfolio.Core.Constants.Enums;
using static AM.Shared.Models.Constants.Enums;
using static Net.Shared.Persistence.Models.Constants.Enums;

namespace AM.Portfolio.Core.Services.DataHeapServices.Bcs.Companies;

public sealed class BcsCompaniesMapper : IBcsCompaniesMapper
{
    private const int AssetTypeId = (int)AssetTypes.Stock;

    private static readonly Dictionary<string, int> DerivativeMap = new();
    private static readonly Dictionary<string, int> AssetMap = new();

    private readonly IAssetRepository _assetRepository;
    private readonly IDerivativeRepository _derivativeRepository;

    public BcsCompaniesMapper(
        IAssetRepository assetRepository,
        IDerivativeRepository derivativeRepository)
    {
        _assetRepository = assetRepository;
        _derivativeRepository = derivativeRepository;
    }

    public async Task Map(BcsCompaniesResult result, CancellationToken cToken)
    {
        var assets = await _assetRepository.Get(AssetTypeId, cToken);

        for (var i = 0; i < assets.Length; i++)
        {
            var asset = assets[i];
            AssetMap.TryAdd(asset.Name, asset.Id);
        }

        var derivatives = await _derivativeRepository.Get(assets.Select(x => x.Id), cToken);

        for (var i = 0; i < derivatives.Length; i++)
        {
            var derivative = derivatives[i];

            DerivativeMap.TryAdd(derivative.Ticker, derivative.AssetId);

            if (derivative.Code is not null)
                DerivativeMap.TryAdd(derivative.Code, derivative.AssetId);
        }

        foreach (var company in result.Companies)
        {
            if (DerivativeMap.ContainsKey(company.Ticker))
                continue;

            if (company.Code is not null && DerivativeMap.ContainsKey(company.Code))
                continue;

            if (!AssetMap.TryGetValue(company.Name, out var assetId))
            {
                var asset = new Asset()
                {
                    Name = company.Name,
                    TypeId = AssetTypeId,

                    Label = company.Ticker,

                    StepId = (int)ProcessSteps.None,
                    StatusId = (int)ProcessStatuses.Draft
                };

                await _assetRepository.Create(asset, cToken);

                assetId = asset.Id;
            }

            var derivative = new Derivative()
            {
                AssetId = assetId,
                Ticker = company.Ticker,
                Code = company.Code,
                ZoneId = (int)Zones.Rus,
                Description = company.Name,

                StepId = (int)ProcessSteps.None,
                StatusId = (int)ProcessStatuses.Draft
            };

            await _derivativeRepository.Create(derivative, cToken);
        }
    }
}
