using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Abstractions.Web;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Computing.Assets;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Loading.Assets;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Sending.Assets;

using Microsoft.Extensions.Logging;
using Shared.Background.Abstractions.EntityState;
using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Services.EntityState.Handlers;

public sealed class AssetStateHandler : EntityStateHandler<Asset>
{
    public AssetStateHandler(
        ILogger<AssetStateHandler> logger
        , IAssetRepository assetRepository
        , IMoexWebclient moexWebclient
        , IDerivativeRepository derivativeRepository) : base(assetRepository, new()
        {
            {(int)Enums.Steps.Loading, new AssetLoader(logger, moexWebclient, derivativeRepository)},
            {(int)Enums.Steps.Computing, new AssetComputer()},
            {(int)Enums.Steps.Sending, new AssetSender()}
        })
    {
    }
}