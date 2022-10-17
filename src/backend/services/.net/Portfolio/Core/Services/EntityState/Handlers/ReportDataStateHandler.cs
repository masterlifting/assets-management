using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Exceptions;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Handling.EntityState;

using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Services.EntityState.Handlers;

public sealed class ReportDataStateHandler : IEntityStateHandler<ReportData>
{
    private readonly Dictionary<int, IEntityStepHandler<ReportData>> _handlers;

    public ReportDataStateHandler(ILogger<ReportDataStateHandler> logger)
    {
        _handlers = new()
        {
            {(int)Enums.Steps.Parsing, new ReportDataParser(logger)}};
    }
    public Task HandleDataAsync(IEntityStepType step, IEnumerable<ReportData> data, CancellationToken cToken) => _handlers.ContainsKey(step.Id)
        ? _handlers[step.Id].HandleAsync(data, cToken)
        : throw new PortfolioCoreException(nameof(ReportDataStateHandler), nameof(HandleDataAsync), Actions.EntityState.StepNotImplemented(step.Name));
}