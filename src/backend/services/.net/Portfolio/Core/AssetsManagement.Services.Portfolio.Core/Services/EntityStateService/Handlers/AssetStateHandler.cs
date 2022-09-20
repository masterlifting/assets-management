using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Abstractions.Web;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Computing.Assets;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Loading.Assets;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Sending.Assets;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Handling.EntityState;
using Shared.Persistense.Exceptions;

using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.Handlers;

public sealed class AssetStateHandler : IEntityStateHandler<Asset>
{
    private readonly Dictionary<int, IEntityStepHandler<Asset>> _handlers;

    public AssetStateHandler(
        ILogger<AssetStateHandler> logger
        , IMoexWebclient moexWebclient
        , IAccountRepository accountRepository
        , IDerivativeRepository derivativeRepository
        , IReportFileRepository reportRepository
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
        : throw new SharedPersistenseEntityStateException(nameof(AssetStateHandler), nameof(HandleDataAsync), $"Запрашиваемый шаг '{step.Name}' для обработки не реализован");
}