using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Computing.Derivatives;
using Microsoft.Extensions.Logging;
using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Handling.EntityState;
using Shared.Persistense.Exceptions;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.Handlers;

public sealed class DerivativeStateHandler : IEntityStateHandler<Derivative>
{
    private readonly Dictionary<int, IEntityStepHandler<Derivative>> _handlers;

    public DerivativeStateHandler(
        ILogger<DerivativeStateHandler> logger
        , IAccountRepository accountRepository
        , IDerivativeRepository derivativeRepository
        , IReportFileRepository reportRepository
        , IDealRepository dealRepository
        , IEventRepository eventRepository)
    {
        _handlers = new()
    {
        {(int)Shared.Persistense.Constants.Enums.Steps.Computing, new DerivativeCalculator()}
    };
    }
    public Task HandleDataAsync(IEntityStepType step, IEnumerable<Derivative> data, CancellationToken cToken) => _handlers.ContainsKey(step.Id)
        ? _handlers[step.Id].HandleAsync(data, cToken)
        : throw new SharedPersistenseEntityStateException(nameof(AssetStateHandler), nameof(HandleDataAsync), $"Запрашиваемый шаг '{step.Name}' для обработки не реализован");
}