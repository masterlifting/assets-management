using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Calculating.Events;

using Microsoft.Extensions.Logging;

using Shared.Infrastructure.Persistense.Abstractions.Entities.State.Handle;
using Shared.Infrastructure.Persistense.Exceptions;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.PipelineHandlers;

public class EventPipelineHandler : IEntityStatePipelineHandler<Event>
{
    private readonly Dictionary<int, IEntityStateStepHandler<Event>> _handlers;

    public EventPipelineHandler(
        ILogger logger
        , IAccountRepository accountRepository
        , IDerivativeRepository derivativeRepository
        , IReportRepository reportRepository
        , IDealRepository dealRepository
        , IEventRepository eventRepository)
    {
        _handlers = new()
        {
            {(int)Domain.Persistense.Entities.Enums.Steps.Calculating, new EventCalculator()}
        };
    }
    public Task HandleDataAsync(int stepId, IEnumerable<Event> data, CancellationToken cToken) => _handlers.ContainsKey(stepId)
        ? _handlers[stepId].HandleAsync(data, cToken)
        : throw new PersistenseEntityStateException("", "", "");
}