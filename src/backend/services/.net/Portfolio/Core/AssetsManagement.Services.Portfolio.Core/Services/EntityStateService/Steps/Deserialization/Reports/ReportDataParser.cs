using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Deserialization.Reports.Bcs;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Handling.EntityState;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Deserialization.Reports;

public sealed class ReportDataParser: IEntityStepHandler<ReportData>
{
    private readonly ILogger _logger;
    private readonly IDerivativeRepository _derivativeRepository;
    private readonly IReportRepository _reportRepository;
    private readonly IDealRepository _dealRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IAccountRepository _accountRepository;

    public ReportDataParser(
        ILogger logger
        , IAccountRepository accountRepository
        , IDerivativeRepository derivativeRepository
        , IReportRepository reportRepository
        , IDealRepository dealRepository
        , IEventRepository eventRepository)

    {
        _logger = logger;
        _derivativeRepository = derivativeRepository;
        _reportRepository = reportRepository;
        _dealRepository = dealRepository;
        _eventRepository = eventRepository;
        _accountRepository = accountRepository;
    }
    public async Task HandleAsync(IEnumerable<ReportData> entities, CancellationToken cToken)
    {
        foreach (var reportFilesGroup in entities.GroupBy(x => x.ProviderId))
            switch (reportFilesGroup.Key)
            {
                case (int)Constants.Persistense.Enums.Providers.Bcs:
                {
                    var parser = new BcsReportDataParser(
                        _logger
                        , _accountRepository
                        , _derivativeRepository
                        , _reportRepository
                        , _dealRepository
                        , _eventRepository);

                    await parser.StartAsync(reportFilesGroup, cToken);
                }
                    break;
            }
    }
}