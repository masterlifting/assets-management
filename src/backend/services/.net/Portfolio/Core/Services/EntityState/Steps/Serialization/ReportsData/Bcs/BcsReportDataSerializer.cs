using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;
using AM.Services.Portfolio.Core.Exceptions;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData.Bcs.Models;

using Microsoft.Extensions.Logging;
using Shared.Extensions.Logging;
using Shared.Extensions.Serialization;
using Shared.Persistense.Exceptions;
using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;
using static Shared.Persistense.Constants.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Serialization.ReportsData.Bcs;

public sealed class BcsReportDataSerializer
{
    private static readonly ProviderId ProviderId = new(Providers.Bcs);

    private readonly ILogger _logger;
    private readonly IAccountRepository _accountRepository;
    private readonly IDerivativeRepository _derivativeRepository;
    private readonly IReportRepository _reportRepository;
    private readonly IDealRepository _dealRepository;
    private readonly IEventRepository _eventRepository;

    public BcsReportDataSerializer(
        ILogger logger
        , IAccountRepository accountRepository
        , IDerivativeRepository derivativeRepository
        , IReportRepository reportRepository
        , IDealRepository dealRepository
        , IEventRepository eventRepository)
    {
        _logger = logger;
        _accountRepository = accountRepository;
        _derivativeRepository = derivativeRepository;
        _reportRepository = reportRepository;
        _dealRepository = dealRepository;
        _eventRepository = eventRepository;
    }

    public async Task StartAsync(IEnumerable<ReportData> files, CancellationToken cToken)
    {
        var derivativeDictionary = await _derivativeRepository.GetGroupedDerivativesAsync(cToken);
        var accountDictionary = await _accountRepository.GetGroupedAccountsByProviderAsync(ProviderId, cToken);
        
        var reports = await GetBcsReportsAsync(files, accountDictionary, derivativeDictionary, cToken);

        await SaveBcsReportsDataAsync(reports.Where(x => x is not null), cToken);
    }

    private Task<BcsReport?[]> GetBcsReportsAsync(IEnumerable<ReportData> files, IDictionary<string, int> accountDictionary, IDictionary<string, string[]> derivativeDictionary, CancellationToken cToken)
        => Task.WhenAll(files
            .Where(file => file.Json is not null)
            .Select(file => Task.Run(() =>
            {
                try
                {
                    var reportModel = file.Json!.Deserialize<BcsReportModel>();
                    
                    var userId = new UserId(file.UserId);

                    var reports = new BcsReport(_logger, file, userId, reportModel, accountDictionary, derivativeDictionary);

                    reports.SetDeals();
                    reports.SetEvents();

                    return reports;
                }
                catch (Exception exception)
                {
                    file.StateId = (int)States.Error;
                    file.Info = exception.Message;
                    
                    return null;
                }
            }, cToken)));
    private async Task SaveBcsReportsDataAsync(IEnumerable<BcsReport> bcsReports, CancellationToken cToken)
    {
        foreach (var reportsGroup in bcsReports.GroupBy(x => x.AccountId.AsInt))
        {
            var accountId = reportsGroup.Key;

            var currentReports = new List<Report>(reportsGroup.Count());

            foreach (var bcsReport in reportsGroup)
            {
                var report = new Report
                {
                    AccountId = accountId,
                    ReportDataId = bcsReport.File.Id,
                    DateStart = bcsReport.DateStart,
                    DateEnd = bcsReport.DateEnd
                };

                var reportCreatedResult = await _reportRepository.TryCreateAsync(report, cToken);

                if (!reportCreatedResult.IsSuccess)
                {
                    bcsReport.File.StateId = (int)States.Error;
                    bcsReport.File.Info = reportCreatedResult.Error;

                    continue;
                }

                currentReports.Add(report);
            }

            if (!currentReports.Any())
            {
                _logger.LogWarn(nameof(BcsReportDataSerializer), nameof(SaveBcsReportsDataAsync), "Reports was not saved");
                continue;
            }

            try
            {
                var reportsDateStart = currentReports.Min(x => x.DateStart);

                var dbReportsDates = await _reportRepository.GetReportsDatesAsync(accountId, reportsDateStart, cToken);
                var currentReportsDates = currentReports
                    .Select(x => ValueTuple.Create(x.DateStart, x.DateEnd))
                    .ToArray();

                var reportsAlreadyDates = GetReportAlreadyDates(dbReportsDates.Concat(currentReportsDates));

                var deals = reportsGroup
                    .SelectMany(x => x!.Deals)
                    .Where(x => !reportsAlreadyDates.Contains(x.Date))
                    .ToArray();

                if (deals.Any())
                    await _dealRepository.CreateRangeAsync(deals, cToken);

                var events = reportsGroup
                    .SelectMany(x => x!.Events)
                    .Where(x => !reportsAlreadyDates.Contains(x.Date))
                    .ToArray();

                if (events.Any())
                    await _eventRepository.CreateRangeAsync(events, cToken);
            }
            catch (Exception exception)
            {
                foreach (var bcsReport in reportsGroup.Join(currentReports, x => x!.File.Id, y => y.ReportDataId, (x, _) => x))
                {
                    bcsReport!.File.StateId = (int)States.Error;
                    bcsReport.File.Info = exception.Message;
                }
            }
        }
    }

    private static DateOnly[] GetReportAlreadyDates(IEnumerable<(DateOnly dateStart, DateOnly dateEnd)> reportDates) => reportDates
        .SelectMany(x =>
        {
            var dateStartTemp = x.dateStart;

            var daysCount = x.dateEnd.DayNumber - x.dateStart.DayNumber;
            var dates = new List<DateOnly>(daysCount) { dateStartTemp };
            while (daysCount > 0)
            {
                dateStartTemp = dateStartTemp.AddDays(1);
                dates.Add(dateStartTemp);
                daysCount--;
            }

            return dates;
        })
        .Distinct()
        .ToArray();
}