using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Deserialization.Reports;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Abstractions.Handling.EntityState;
using Shared.Persistense.Exceptions;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.Handlers
{
    public sealed class ReportDataStateHandler : IEntityStateHandler<ReportData>
    {
        private readonly Dictionary<int, IEntityStepHandler<ReportData>> _handlers;

        public ReportDataStateHandler(
            ILogger<ReportDataStateHandler> logger
            , IAccountRepository accountRepository
            , IDerivativeRepository derivativeRepository
            , IReportRepository reportRepository
            , IDealRepository dealRepository
            , IEventRepository eventRepository)
        {
            _handlers = new()
        {
            {(int)Shared.Persistense.Constants.Enums.Steps.Parsing, new ReportDataParser(
                logger
                , accountRepository
                , derivativeRepository
                , reportRepository
                , dealRepository
                , eventRepository)}
            };
        }
        public Task HandleDataAsync(IEntityStepType step, IEnumerable<ReportData> data, CancellationToken cToken) => _handlers.ContainsKey(step.Id)
            ? _handlers[step.Id].HandleAsync(data, cToken)
            : throw new SharedPersistenseEntityStateException(nameof(AssetStateHandler), nameof(HandleDataAsync), $"Запрашиваемый шаг '{step.Name}' для обработки не реализован");
    }
}