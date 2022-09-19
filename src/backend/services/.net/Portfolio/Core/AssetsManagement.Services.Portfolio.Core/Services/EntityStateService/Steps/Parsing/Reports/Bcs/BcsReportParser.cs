using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Enums;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

using Microsoft.Extensions.Logging;

using Shared.Extensions.Logging;
using Shared.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Parsing.Reports.Bcs;

public class BcsReportParser
{
    private static readonly ProviderId ProviderId = new(Providers.Bcs);

    private readonly ILogger _logger;
    private readonly IAccountRepository _accountRepository;
    private readonly IDerivativeRepository _derivativeRepository;
    private readonly IReportRepository _reportRepository;
    private readonly IDealRepository _dealRepository;
    private readonly IEventRepository _eventRepository;

    public BcsReportParser(
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

    public async Task StartAsync(IEnumerable<Report> reports, CancellationToken cToken)
    {
        var derivativeDictionary = await _derivativeRepository.GetGroupedDerivativesAsync(cToken);
        var bcsReports = await GetBcsReportsAsync(reports, derivativeDictionary, cToken);

        await ParseHeadersAsync(bcsReports, cToken);

        var accountDictionary = await _accountRepository.GetGroupedAccountsByProviderAsync(ProviderId, cToken);
        await ParseBodiesAsync(bcsReports, accountDictionary, cToken);

        await SaveBcsReportsDataAsync(bcsReports, cToken);
    }

    private Task<BcsReport?[]> GetBcsReportsAsync(IEnumerable<Report> reports, IDictionary<string, string[]> derivativeDictionary, CancellationToken cToken)
        => Task.WhenAll(reports
            .Select(report => Task.Run(() =>
            {
                try
                {
                    return new BcsReport(_logger, report, derivativeDictionary);
                }
                catch (Exception exception)
                {
                    report.StateId = (int)States.Error;
                    report.Info = exception.Message;
                    return null;
                }
            }, cToken)));
    private static Task ParseHeadersAsync(IEnumerable<BcsReport?> bcsReports, CancellationToken cToken)
        => Task.WhenAll(bcsReports
            .Where(bcsReport => bcsReport is not null)
            .Select(bcsReport => Task.Run(() =>
            {
                try
                {
                    bcsReport!.Header = bcsReport.GetHeader();
                    bcsReport.Report.DateStart = bcsReport.Header.DateStart;
                    bcsReport.Report.DateEnd = bcsReport.Header.DateEnd;
                }
                catch (Exception exception)
                {
                    bcsReport!.Report.StateId = (int)States.Error;
                    bcsReport.Report.Info = exception.Message;
                }
            }, cToken)));
    private Task ParseBodiesAsync(IEnumerable<BcsReport?> bcsReports, IDictionary<string, int> accountDictionary, CancellationToken cToken)
        => Task.WhenAll(bcsReports
            .Where(bcsReport => bcsReport?.Header is not null)
            .GroupBy(bcsReport => bcsReport!.Header!.Agreement)
            .Select(groupBcsReports => Task.Run(async () =>
            {
                #region automatically creating account!!!
                if (!accountDictionary.ContainsKey(groupBcsReports.Key))
                {
                    var accounts = groupBcsReports
                        .Select(x => new Account
                        {
                            Name = groupBcsReports.Key,
                            UserId = x!.Report.UserId,
                            ProviderId = ProviderId.AsInt,
                        })
                        .ToArray();

                    await _accountRepository.CreateRangeAsync(accounts, cToken);

                    foreach (var account in accounts)
                    {
                        accountDictionary.Add(account.Name, account.Id);
                        _logger.LogWarn("Парсер БКС отчетов", "Автоматическое создание аккаунта", account.Name);
                    }
                }
                #endregion

                var accountId = accountDictionary[groupBcsReports.Key];

                await Task.WhenAll(groupBcsReports.Select(groupBcsReport => Task.Run(() =>
                {
                    try
                    {
                        groupBcsReport!.Body = groupBcsReport.GetBody(accountId);
                        groupBcsReport.Report.AccountId = accountId;
                    }
                    catch (Exception exception)
                    {
                        groupBcsReport!.Report.StateId = (int)States.Error;
                        groupBcsReport.Report.Info = exception.Message;
                    }
                }, cToken)));
            }, cToken)));
    private async Task SaveBcsReportsDataAsync(IEnumerable<BcsReport?> bcsReports, CancellationToken cToken)
    {
        foreach (var group in bcsReports
                     .Where(x => x?.Header is not null && x.Body is not null)
                     .GroupBy(x => x!.Body!.AccountId))
        {
            var accountId = group.Key;
            var reports = group.Select(x => x!.Report).ToArray();

            try
            {
                var reportsDateStart = reports.Min(x => x.DateStart!.Value);

                var dbReportDates = await _reportRepository.GetReportDatesAsync(accountId, ProviderId, reportsDateStart, cToken);
                var currentReportDates = reports
                    .Select(x => ValueTuple.Create(x.DateStart!.Value, x.DateEnd!.Value))
                    .ToArray();

                var reportAlreadyDates = GetReportAlreadyDates(dbReportDates.Concat(currentReportDates));

                var deals = group
                    .SelectMany(x => x!.Body!.Deals)
                    .Where(x => !reportAlreadyDates.Contains(x.Date))
                    .ToArray();

                if (deals.Any())
                    await _dealRepository.CreateRangeAsync(deals, cToken);

                var events = group
                    .SelectMany(x => x!.Body!.Events)
                    .Where(x => !reportAlreadyDates.Contains(x.Date))
                    .ToArray();

                if (events.Any())
                    await _eventRepository.CreateRangeAsync(events, cToken);
            }
            catch (Exception exception)
            {
                foreach (var report in reports)
                {
                    report.StateId = (int)States.Error;
                    report.Info = exception.Message;
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