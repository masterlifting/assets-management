using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Exceptions;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Computing.Derivatives;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Handling.EntityState;

using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Services.EntityState.Handlers;

public sealed class DerivativeStateHandler : IEntityStateHandler<Derivative>
{
    private readonly Dictionary<int, IEntityStepHandler<Derivative>> _handlers;

    public DerivativeStateHandler(
        ILogger<DerivativeStateHandler> logger
        , IAccountRepository accountRepository
        , IDerivativeRepository derivativeRepository
        , IReportDataRepository reportRepository
        , IDealRepository dealRepository
        , IEventRepository eventRepository)
    {
        _handlers = new()
{
    {(int)Enums.Steps.Computing, new DerivativeCalculator()}
};
    }
    public Task HandleDataAsync(IEntityStepType step, IEnumerable<Derivative> data, CancellationToken cToken) => _handlers.ContainsKey(step.Id)
        ? _handlers[step.Id].HandleAsync(data, cToken)
        : throw new PortfolioCoreException(nameof(DerivativeStateHandler), nameof(HandleDataAsync), Actions.EntityState.StepNotImplemented(step.Name));
}