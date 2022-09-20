using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Calculating.Assets;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Sending.Assets;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Handling.EntityState;
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
        {(int)Shared.Persistense.Constants.Enums.Steps.Calculating, new AssetCalculator()},
        {(int)Shared.Persistense.Constants.Enums.Steps.Sending, new AssetSender()}
    };
    }
    public Task HandleDataAsync(IEntityStepCatalog step, IEnumerable<Asset> data, CancellationToken cToken) => _handlers.ContainsKey(step.Id)
        ? _handlers[step.Id].HandleAsync(data, cToken)
        : throw new SharedPersistenseEntityStateException("", "", "");
}