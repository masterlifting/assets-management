using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Deserialization.Reports;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Handling.EntityState;
using Shared.Persistense.Exceptions;

namespace AM.Services.Portfolio.Core.Services.EntityState.Handlers
{
    public sealed class ReportDataStateHandler : IEntityStateHandler<ReportData>
    {
        private readonly Dictionary<int, IEntityStepHandler<ReportData>> _handlers;

        public ReportDataStateHandler(ILogger<ReportDataStateHandler> logger)
        {
            _handlers = new()
            {
                {(int)Shared.Persistense.Constants.Enums.Steps.Parsing, new ReportDataParser()}};
            }
        public Task HandleDataAsync(IEntityStepType step, IEnumerable<ReportData> data, CancellationToken cToken) => _handlers.ContainsKey(step.Id)
            ? _handlers[step.Id].HandleAsync(data, cToken)
            : throw new SharedPersistenseEntityStateException(nameof(ReportDataStateHandler), nameof(HandleDataAsync), $"Запрашиваемый шаг '{step.Name}' для обработки не реализован");
    }
}