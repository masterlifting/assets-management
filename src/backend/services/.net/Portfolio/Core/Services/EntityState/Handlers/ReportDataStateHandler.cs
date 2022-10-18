using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Serialization.ReportsData;

using Microsoft.Extensions.Logging;
using Shared.Background.Abstractions.EntityState;
using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Services.EntityState.Handlers;

public sealed class ReportDataStateHandler : EntityStateHandler<ReportData>
{
    public ReportDataStateHandler(
        ILogger<ReportDataStateHandler> logger
        , IReportDataRepository reportDataRepository
        , IAccountRepository accountRepository
        , IDerivativeRepository derivativeRepository
        , IReportRepository reportRepository
        , IDealRepository dealRepository
        , IEventRepository eventRepository) : base(reportDataRepository, new()
        {
            {(int)Enums.Steps.Parsing, new ReportDataParser(logger)},
            {(int)Enums.Steps.Serialization, new ReportDataSerializer(logger,accountRepository,derivativeRepository,reportRepository,dealRepository,eventRepository)}
        })
    {
    }
}