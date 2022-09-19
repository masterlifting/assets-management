using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Calculating.Deals;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities.State;
using Shared.Persistense.Abstractions.Entities.State.Handle;
using Shared.Persistense.Exceptions;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.PipelineHandlers;

public class DealPipelineHandler : IEntityStatePipelineHandler<Deal>
{
    private readonly Dictionary<int, IEntityStateStepHandler<Deal>> _handlers;

    public DealPipelineHandler(
        ILogger logger
        , IAccountRepository accountRepository
        , IDerivativeRepository derivativeRepository
        , IReportRepository reportRepository
        , IDealRepository dealRepository
        , IEventRepository eventRepository)
    {
        _handlers = new()
        {
            {(int)Domain.Persistense.Entities.Enums.Steps.Calculating, new DealCalculator()}
        };
    }
    public Task HandleDataAsync(IEntityStep step, IEnumerable<Deal> data, CancellationToken cToken) => _handlers.ContainsKey(step.Id)
        ? _handlers[step.Id].HandleAsync(data, cToken)
        : throw new SharedPersistenseEntityStateException("", "", "");
}