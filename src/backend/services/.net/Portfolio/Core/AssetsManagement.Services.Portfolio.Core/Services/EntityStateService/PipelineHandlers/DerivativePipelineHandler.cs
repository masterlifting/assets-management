using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Calculating.Derivatives;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Handling.EntityState;
using Shared.Persistense.Exceptions;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.PipelineHandlers;

public class DerivativePipelineHandler : IEntityStatePipelineHandler<Derivative>
{
    private readonly Dictionary<int, IEntityStateStepHandler<Derivative>> _handlers;

    public DerivativePipelineHandler(
        ILogger logger
        , IAccountRepository accountRepository
        , IDerivativeRepository derivativeRepository
        , IReportRepository reportRepository
        , IDealRepository dealRepository
        , IEventRepository eventRepository)
    {
        _handlers = new()
    {
        {(int)Shared.Persistense.Constants.Enums.Steps.Calculating, new DerivativeCalculator()}
    };
    }
    public Task HandleDataAsync(IEntityStepCatalog step, IEnumerable<Derivative> data, CancellationToken cToken) => _handlers.ContainsKey(step.Id)
        ? _handlers[step.Id].HandleAsync(data, cToken)
        : throw new SharedPersistenseEntityStateException("", "", "");
}