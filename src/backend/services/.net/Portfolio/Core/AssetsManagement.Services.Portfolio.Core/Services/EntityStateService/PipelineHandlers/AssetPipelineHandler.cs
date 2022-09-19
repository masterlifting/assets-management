using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Calculating.Assets;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Sending.Assets;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities.State.Handle;
using Shared.Persistense.Exceptions;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.PipelineHandlers;

public class AssetPipelineHandler : IEntityStatePipelineHandler<Asset>
{
    private readonly Dictionary<int, IEntityStateStepHandler<Asset>> _handlers;

    public AssetPipelineHandler(
        ILogger logger
        , IAccountRepository accountRepository
        , IDerivativeRepository derivativeRepository
        , IReportRepository reportRepository
        , IDealRepository dealRepository
        , IEventRepository eventRepository)
    {
        _handlers = new()
        {
            {(int)Domain.Persistense.Entities.Enums.Steps.Calculating, new AssetCalculator()},
            {(int)Domain.Persistense.Entities.Enums.Steps.Sending, new AssetSender()}
        };
    }
    public Task HandleDataAsync(int stepId, IEnumerable<Asset> data, CancellationToken cToken) => _handlers.ContainsKey(stepId)
        ? _handlers[stepId].HandleAsync(data, cToken)
        : throw new PersistenseEntityStateException("", "", "");
}