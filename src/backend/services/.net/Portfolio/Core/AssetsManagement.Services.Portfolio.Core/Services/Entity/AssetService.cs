using AM.Services.Common.Contracts.Dto;
using AM.Services.Portfolio.Core.Abstractions.External.Webclients;
using AM.Services.Portfolio.Core.Abstractions.Persistense.UnitOfWorks;
using AM.Services.Portfolio.Core.Domain.Persistense.Models;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

using Microsoft.Extensions.Logging;

using Shared.Extensions.Logging;

namespace AM.Services.Portfolio.Core.Services.Entity;

public class AssetService
{
    private const string Initiator = "Добавление активов";

    private readonly ILogger<AssetService> _logger;
    private readonly IUnitOfWorkRepository _unitOfWork;
    private readonly IMoexWebclient _moexWebclient;

    public AssetService(ILogger<AssetService> logger, IMoexWebclient moexWebclient, IUnitOfWorkRepository unitOfWork)
    {
        _logger = logger;
        _moexWebclient = moexWebclient;
        _unitOfWork = unitOfWork;
    }

    public async Task SetAssetAndDerivativeAsync(AssetDto dto)
    {
        var countryId = new CountryId(dto.CountryId);
        var assetId = new AssetId(dto.AssetId);
        var assetTypeId = new AssetTypeId(dto.AssetTypeId);

        var moexData = await _moexWebclient.GetIsinAsync(assetId.AsString, countryId.AsEnum);

        var isinIndex = countryId.AsEnum == Common.Contracts.Entities.Enums.Countries.Rus ? 19 : 18;

        var isin = moexData.Securities.Data[0][isinIndex].ToString();
        
        var derivativeId = new DerivativeId(isin);
        var derivativeCode = new DerivativeCode(dto.AssetId);

        var assetModel = new AssetModel(assetId, assetTypeId, countryId, dto.Name, $"Создание актива по значению: {dto.AssetId}");

        try
        {
            await _unitOfWork.Asset.CreateAsync(assetModel.GetEntity());
        }
        catch (Exception exception)
        {
            _logger.LogError(Initiator, "Создание актива", exception);
        }

        var derivativeModel = new DerivativeModel(derivativeId, derivativeCode, assetId, assetTypeId, 0);

        try
        {
            await _unitOfWork.Derivative.CreateAsync(derivativeModel.GetEntity());
        }
        catch (Exception exception)
        {
            _logger.LogError(Initiator, "Создание дериватива", exception);
        }
    }
    public async Task SetAssetsAndDerivativesAsync(IEnumerable<AssetDto> dtos)
    {
        var assetModels = dtos
            .Select(x => new AssetModel(
                new AssetId(x.AssetId)
                , new AssetTypeId(x.AssetTypeId)
                , new CountryId(x.CountryId)
                , x.Name
                , null))
            .ToArray();

        var assets = await _unitOfWork.Asset.GetNewAssetsAsync(assetModels);

        if (assets.Any())
            await _unitOfWork.Asset.CreateRangeAsync(assets);
        else
            return;

        var derivativeModels = new List<DerivativeModel>(assets.Length);

        foreach (var group in assetModels.GroupBy(x => x.CountryId.AsEnum))
        {
            var country = group.Key;

            var assetData = new Dictionary<string, AssetModel>(group.Count());

            if (country == Common.Contracts.Entities.Enums.Countries.Rus)
                foreach (var item in group)
                    assetData.Add(item.AssetId.AsString, item);
            else
                foreach (var item in group)
                    assetData.Add($"{item.AssetId.AsString}-RM", item);

            var isinIndex = country == Common.Contracts.Entities.Enums.Countries.Rus ? 19 : 18;

            var response = await _moexWebclient.GetIsinsAsync(country);

            var moexData = response.Securities.Data
                .Select(x => (Ticker: x[0].ToString(), Isin: x[isinIndex].ToString()))
                .ToArray();

            foreach (var (ticker, isin, assetModel) in moexData.Join(assetData
                         , x => x.Ticker
                         , y => y.Key
                         , (x, y) => (x.Ticker, x.Isin, y.Value)))
            {
                if (string.IsNullOrWhiteSpace(isin) || isin.Equals("0"))
                {
                    _logger.LogWarn(Initiator, "Определение ISIN для дериватива", "Не расопзнано", ticker);
                    continue;
                }

                var derivativeModel = new DerivativeModel(new DerivativeId(isin), new DerivativeCode(ticker), assetModel.AssetId, assetModel.AssetTypeId, 0);
                derivativeModels.Add(derivativeModel);
            }
        }

        if (derivativeModels.Any())
            await _unitOfWork.Derivative.CreateRangeAsync(derivativeModels.Select(x => x.GetEntity()).ToArray());
    }
}