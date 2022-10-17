using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Exceptions;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Computing.Events;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Handling.EntityState;

using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Services.EntityState.Handlers;

public sealed class EventStateHandler : IEntityStateHandler<Event>
{
    private readonly Dictionary<int, IEntityStepHandler<Event>> _handlers;

    public EventStateHandler(
        ILogger<EventStateHandler> logger
        , IAccountRepository accountRepository
        , IDerivativeRepository derivativeRepository
        , IReportDataRepository reportRepository
        , IDealRepository dealRepository
        , IEventRepository eventRepository)
    {
        _handlers = new()
    {
        {(int)Enums.Steps.Computing, new EventCalculator()}
    };
    }
    public Task HandleDataAsync(IEntityStepType step, IEnumerable<Event> data, CancellationToken cToken) => _handlers.ContainsKey(step.Id)
        ? _handlers[step.Id].HandleAsync(data, cToken)
        : throw new PortfolioCoreException(nameof(EventStateHandler), nameof(HandleDataAsync), Actions.EntityState.StepNotImplemented(step.Name));
}