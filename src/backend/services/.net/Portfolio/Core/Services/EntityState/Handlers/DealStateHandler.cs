using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Exceptions;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Computing.Deals;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Handling.EntityState;

using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Services.EntityState.Handlers;

public sealed class DealStateHandler : IEntityStateHandler<Deal>
{
    private readonly Dictionary<int, IEntityStepHandler<Deal>> _handlers;

    public DealStateHandler(
        ILogger<DealStateHandler> logger
        , IAccountRepository accountRepository
        , IDerivativeRepository derivativeRepository
        , IReportDataRepository reportRepository
        , IDealRepository dealRepository
        , IEventRepository eventRepository)
    {
        _handlers = new()
        {
            {(int)Enums.Steps.Computing, new DealCalculator()}
        };
    }
    public Task HandleDataAsync(IEntityStepType step, IEnumerable<Deal> data, CancellationToken cToken) => _handlers.ContainsKey(step.Id)
        ? _handlers[step.Id].HandleAsync(data, cToken)
        : throw new PortfolioCoreException(nameof(DealStateHandler), nameof(HandleDataAsync), Actions.EntityState.StepNotImplemented(step.Name));
}