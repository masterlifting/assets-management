using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

using Microsoft.Extensions.Logging;

using Shared.Extensions.Logging;
using Shared.Persistense.Exceptions;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;
using static Shared.Persistense.Constants.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Serialization.Reports.Bcs
{
    public sealed class BcsReportParser
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

        public async Task StartAsync(IEnumerable<ReportData> files, CancellationToken cToken)
        {
            var derivativeDictionary = await _derivativeRepository.GetGroupedDerivativesAsync(cToken);
            var reports = await GetBcsReportsAsync(files, derivativeDictionary, cToken);

            await ParseHeadersAsync(reports, cToken);

            var accountDictionary = await _accountRepository.GetGroupedAccountsByProviderAsync(ProviderId, cToken);
            await ParseBodiesAsync(reports, accountDictionary, cToken);

            await SaveBcsReportsDataAsync(reports, cToken);
        }

        private Task<BcsReport?[]> GetBcsReportsAsync(IEnumerable<ReportData> files, IDictionary<string, string[]> derivativeDictionary, CancellationToken cToken)
            => Task.WhenAll(files
                .Select(file => Task.Run(() =>
                {
                    try
                    {
                        return new BcsReport(_logger, file, derivativeDictionary);
                    }
                    catch (Exception exception)
                    {
                        file.StateId = (int)States.Error;
                        file.Info = exception.Message;
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
                    }
                    catch (Exception exception)
                    {
                        bcsReport!.File.StateId = (int)States.Error;
                        bcsReport.File.Info = exception.Message;
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
                                UserId = x!.File.UserId,
                                ProviderId = ProviderId.AsInt,
                            })
                            .ToArray();

                        await _accountRepository.CreateRangeAsync(accounts, cToken);

                        foreach (var account in accounts)
                        {
                            accountDictionary.Add(account.Name, account.Id);
                            _logger.LogWarn(nameof(BcsReportParser), "Автоматическое создание акаунта", account.Name);
                        }
                    }
                    #endregion

                    var accountId = accountDictionary[groupBcsReports.Key];

                    await Task.WhenAll(groupBcsReports.Select(groupBcsReport => Task.Run(() =>
                    {
                        try
                        {
                            groupBcsReport!.Body = groupBcsReport.GetBody(accountId);
                        }
                        catch (Exception exception)
                        {
                            groupBcsReport!.File.StateId = (int)States.Error;
                            groupBcsReport.File.Info = exception.Message;
                        }
                    }, cToken)));
                }, cToken)));
        private async Task SaveBcsReportsDataAsync(IEnumerable<BcsReport?> bcsReports, CancellationToken cToken)
        {
            foreach (var reportsGroup in bcsReports
                         .Where(x => x?.Header is not null && x.Body is not null)
                         .GroupBy(x => x!.Body!.AccountId))
            {
                var accountId = reportsGroup.Key;
                var currentReports = new List<Report>(reportsGroup.Count());

                foreach (var bcsReport in reportsGroup)
                {
                    var report = new Report
                    {
                        AccountId = reportsGroup.Key,
                        DateStart = bcsReport!.Header!.DateStart,
                        DateEnd = bcsReport.Header.DateEnd,
                        ReportDataId = bcsReport.File.Id
                    };

                    var reportCreatedResult = await _reportRepository.TryCreateAsync(report, cToken);

                    if (!reportCreatedResult.IsSuccess)
                    {
                        bcsReport.File.StateId = (int)States.Error;
                        bcsReport.File.Info = reportCreatedResult.Error;

                        _logger.LogError(new SharedPersistenseEntityStepException(nameof(BcsReportParser), $"Сохранение отчета БКС '{bcsReport.File.Name}'", reportCreatedResult.Error!));

                        continue;
                    }

                    currentReports.Add(report);
                }

                if (!currentReports.Any())
                {
                    _logger.LogWarn(nameof(BcsReportParser), "Сохранение отчетов БКС", "Не удалось сохранить ни одного отчета");
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
                        .SelectMany(x => x!.Body!.Deals)
                        .Where(x => !reportsAlreadyDates.Contains(x.Date))
                        .ToArray();

                    if (deals.Any())
                        await _dealRepository.CreateRangeAsync(deals, cToken);

                    var events = reportsGroup
                        .SelectMany(x => x!.Body!.Events)
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
}