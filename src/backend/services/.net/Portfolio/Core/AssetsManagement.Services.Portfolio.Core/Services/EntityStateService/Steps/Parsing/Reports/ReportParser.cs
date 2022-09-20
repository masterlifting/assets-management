using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Parsing.Reports.Bcs;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Handling.EntityState;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Parsing.Reports;

public class ReportParser : IEntityStateStepHandler<Report>
{
    private readonly ILogger _logger;
    private readonly IDerivativeRepository _derivativeRepository;
    private readonly IReportRepository _reportRepository;
    private readonly IDealRepository _dealRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IAccountRepository _accountRepository;

    public ReportParser(
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
    public async Task HandleAsync(IEnumerable<Report> entities, CancellationToken cToken)
    {
        foreach (var reports in entities.GroupBy(x => x.ProviderId))
            switch (reports.Key)
            {
                case (int)Providers.Bcs:
                    {
                        var parser = new BcsReportParser(
                            _logger
                            , _accountRepository
                            , _derivativeRepository
                            , _reportRepository
                            , _dealRepository
                            , _eventRepository);
                        await parser.StartAsync(reports, cToken);
                    }
                    break;
            }
    }
}