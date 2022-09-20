using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Parsing.Reports;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Handling.EntityState;
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
        {(int)Shared.Persistense.Constants.Enums.Steps.Parsing, new ReportParser(
            logger
            , accountRepository
            , derivativeRepository
            , reportRepository
            , dealRepository
            , eventRepository)}
    };
    }
    public Task HandleDataAsync(IEntityStepCatalog step, IEnumerable<Report> data, CancellationToken cToken) => _handlers.ContainsKey(step.Id)
        ? _handlers[step.Id].HandleAsync(data, cToken)
        : throw new SharedPersistenseEntityStateException("", "", "");
}