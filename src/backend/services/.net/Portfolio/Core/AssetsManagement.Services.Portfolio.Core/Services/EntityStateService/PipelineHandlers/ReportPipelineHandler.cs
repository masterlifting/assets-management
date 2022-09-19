using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Parsing.Reports;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities.State.Handle;
using Shared.Persistense.Exceptions;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.PipelineHandlers;

public class ReportPipelineHandler : IEntityStatePipelineHandler<Report>
{
    private readonly Dictionary<int, IEntityStateStepHandler<Report>> _handlers;

    public ReportPipelineHandler(
        ILogger logger
        , IAccountRepository accountRepository
        , IDerivativeRepository derivativeRepository
        , IReportRepository reportRepository
        , IDealRepository dealRepository
        , IEventRepository eventRepository)
    {
        _handlers = new()
        {
            {(int)Domain.Persistense.Entities.Enums.Steps.Parsing, new ReportParser(
                logger
                , accountRepository
                , derivativeRepository
                , reportRepository
                , dealRepository
                , eventRepository)}
        };
    }
    public Task HandleDataAsync(int stepId, IEnumerable<Report> data, CancellationToken cToken) => _handlers.ContainsKey(stepId)
        ? _handlers[stepId].HandleAsync(data, cToken)
        : throw new PersistenseEntityStateException("", "", "");
}