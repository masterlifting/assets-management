using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Abstractions.Web;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Exceptions;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Computing.Assets;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Loading.Assets;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Sending.Assets;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Handling.EntityState;

using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Services.EntityState.Handlers;

public sealed class AssetStateHandler : IEntityStateHandler<Asset>
{
    private readonly Dictionary<int, IEntityStepHandler<Asset>> _handlers;

    public AssetStateHandler(
        ILogger<AssetStateHandler> logger
        , IMoexWebclient moexWebclient
        , IAccountRepository accountRepository
        , IDerivativeRepository derivativeRepository
        , IReportDataRepository reportRepository
        , IDealRepository dealRepository
        , IEventRepository eventRepository)
    {
        _handlers = new()
        {
            {(int)Enums.Steps.Loading, new AssetLoader(logger, moexWebclient, derivativeRepository)},
            {(int)Enums.Steps.Computing, new AssetComputer()},
            {(int)Enums.Steps.Sending, new AssetSender()}
        };
    }
    public Task HandleDataAsync(IEntityStepType step, IEnumerable<Asset> data, CancellationToken cToken) => _handlers.ContainsKey(step.Id)
        ? _handlers[step.Id].HandleAsync(data, cToken)
        : throw new PortfolioCoreException(nameof(AssetStateHandler), nameof(HandleDataAsync), Actions.EntityState.StepNotImplemented(step.Name));
}