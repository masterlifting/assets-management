using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Parsing.ReportFiles;
using Microsoft.Extensions.Logging;
using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Handling.EntityState;
using Shared.Persistense.Exceptions;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.Handlers;

public sealed class ReportFileStateHandler : IEntityStateHandler<ReportFile>
{
    private readonly Dictionary<int, IEntityStepHandler<ReportFile>> _handlers;

    public ReportFileStateHandler(
        ILogger<ReportFileStateHandler> logger
        , IAccountRepository accountRepository
        , IDerivativeRepository derivativeRepository
        , IReportRepository reportRepository
        , IDealRepository dealRepository
        , IEventRepository eventRepository)
    {
        _handlers = new()
        {
            {(int)Shared.Persistense.Constants.Enums.Steps.Parsing, new ReportFileParser(
                logger
                , accountRepository
                , derivativeRepository
                , reportRepository
                , dealRepository
                , eventRepository)}
            };
        }
    public Task HandleDataAsync(IEntityStepType step, IEnumerable<ReportFile> data, CancellationToken cToken) => _handlers.ContainsKey(step.Id)
        ? _handlers[step.Id].HandleAsync(data, cToken)
        : throw new SharedPersistenseEntityStateException(nameof(AssetStateHandler), nameof(HandleDataAsync), $"Запрашиваемый шаг '{step.Name}' для обработки не реализован");
}