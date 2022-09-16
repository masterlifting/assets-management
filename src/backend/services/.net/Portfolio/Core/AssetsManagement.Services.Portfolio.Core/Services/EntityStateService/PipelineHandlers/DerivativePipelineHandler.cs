using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Interfaces.Persistense.Repositories;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Calculating.Derivatives;

using Microsoft.Extensions.Logging;

using Shared.Exceptions;
using Shared.Infrastructure.Persistense.Entities.EntityState;

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
            {(int)Domain.Persistense.Entities.Enums.Steps.Calculating, new DerivativeCalculator()}
        };
    }
    public Task HandleDataAsync(int stepId, IEnumerable<Derivative> data, CancellationToken cToken) => _handlers.ContainsKey(stepId)
        ? _handlers[stepId].HandleAsync(data, cToken)
        : throw new SharedProcessException("", "", "");
}