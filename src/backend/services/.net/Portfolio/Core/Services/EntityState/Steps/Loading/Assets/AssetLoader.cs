﻿using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Abstractions.Web;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Domain.Persistense.Models;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;
using AM.Services.Portfolio.Core.Models.Clients;

using Microsoft.Extensions.Logging;

using Shared.Extensions.Logging;
using Shared.Persistense.Abstractions.Handling.EntityState;
using Shared.Persistense.Exceptions;

using static AM.Services.Common.Contracts.Constants.Persistense.Enums;
using static Shared.Persistense.Constants.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Loading.Assets;

public sealed class AssetLoader : IEntityStepHandler<Asset>
{
    private const string Initiator = nameof(AssetLoader);
    private readonly ILogger _logger;
    private readonly IMoexWebclient _moexWebclient;
    private readonly IDerivativeRepository _derivativeRepository;

    public AssetLoader(
        ILogger logger
        , IMoexWebclient moexWebclient
        , IDerivativeRepository derivativeRepository)
    {
        _logger = logger;
        _moexWebclient = moexWebclient;
        _derivativeRepository = derivativeRepository;
    }
    public async Task HandleAsync(IEnumerable<Asset> entities, CancellationToken cToken)
    {
        var assets = entities.ToArray();

        if (!assets.Any())
        {
            _logger.LogWarn(Initiator, nameof(HandleAsync), "Data not found for processing");
            return;
        }

        foreach (var assetsGroup in assets.GroupBy(x => x.CountryId))
        {
            var countryId = new CountryId(assetsGroup.Key);
            var assetDictionary = new Dictionary<string, Asset>(assetsGroup.Count());

            if (countryId.AsEnum == Countries.Rus)
                foreach (var item in assetsGroup)
                    assetDictionary.Add(item.Id, item);
            else
                foreach (var item in assetsGroup)
                    assetDictionary.Add($"{item.Id}-RM", item);

            MoexIsinData moexResponse;

            try
            {
                moexResponse = await _moexWebclient.GetIsinsAsync(countryId.AsEnum);
            }
            catch (Exception exception)
            {
                _logger.LogError(new SharedPersistenseEntityStepException(Initiator, $"Receiving ISIN list by country: {countryId.AsString}", exception));
                foreach (var asset in assetsGroup)
                {
                    asset.StateId = (int)States.Error;
                    asset.Info = exception.Message;
                }
                continue;
            }

            var isinIndex = countryId.AsEnum == Countries.Rus ? 19 : 18;

            var moexData = moexResponse.Securities.Data
                .Select(x => (Ticker: x[0].ToString(), Isin: x[isinIndex].ToString()))
                .ToArray();

            foreach (var (ticker, isin, asset) in moexData.Join(assetDictionary
                         , x => x.Ticker
                         , y => y.Key
                         , (x, y) => (x.Ticker, x.Isin, y.Value)))
            {
                if (string.IsNullOrWhiteSpace(isin) || isin.Equals("0"))
                {
                    asset.StateId = (int)States.Error;
                    asset.Info = $"The ISIN not recognized by: '{ticker}'";

                    _logger.LogError(new SharedPersistenseEntityStepException(Initiator, $"Recognizing the ISIN by '{ticker}'", "Not recognized"));

                    continue;
                }

                var assetModel = AssetModel.GetModel(asset);

                var derivativeModel = new DerivativeModel(
                    new DerivativeId(isin)
                    , new DerivativeCode(ticker)
                    , assetModel.AssetId
                    , assetModel.AssetTypeId
                    , 0);

                var createdResult = await _derivativeRepository.TryCreateAsync(derivativeModel.GetEntity());

                if (createdResult.IsSuccess)
                    continue;

                asset.StateId = (int)States.Error;
                asset.Info = createdResult.Error!;

                _logger.LogError(new SharedPersistenseEntityStepException(Initiator, $"Receiving the derivative by: {asset.Name}", createdResult.Error!));
            }
        }
    }
}