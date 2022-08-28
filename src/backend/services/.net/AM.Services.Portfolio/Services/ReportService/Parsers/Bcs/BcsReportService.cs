using AM.Services.Portfolio.Domain.DataAccess;
using AM.Services.Portfolio.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static AM.Services.Portfolio.Enums;
using static Shared.Contracts.Enums;

namespace AM.Services.Portfolio.Services.ReportService.Parsers.Bcs;

public class BcsReportService
{
    private const int ProviderId = (int)Providers.BCS;

    private readonly ILogger _logger;
    private readonly DatabaseContext _context;
    private readonly Dictionary<string, int> _accountsDictionary;
    private readonly Dictionary<string, string[]> _derivativesDictionary;

    public BcsReportService(
        ILogger logger
        , DatabaseContext context
        , Dictionary<string, int> accountsDictionary
        , Dictionary<string, string[]> derivativesDictionary)
    {
        _logger = logger;
        _context = context;
        _accountsDictionary = accountsDictionary;
        _derivativesDictionary = derivativesDictionary;
    }

    public async Task ParseAsync(IEnumerable<ReportFile> reportFiles, CancellationToken cToken)
    {
        var bcsReports = await Task.WhenAll(reportFiles.Select(reportFile => Task.Run(() =>
        {
            try
            {
                return new BcsReport(_logger, reportFile, _derivativesDictionary);
            }
            catch (Exception exception)
            {
                reportFile.StateId = (byte)States.Error;
                reportFile.Info = exception.Message;
                return null;
            }
        }, cToken)));

        var bcsrReportsWithHeaders = await Task.WhenAll(bcsReports
            .Where(bcsReport => bcsReport is not null)
            .Select(bcsReport => Task.Run(() =>
            {
                try
                {
                    bcsReport!.Header = bcsReport.GetHeader();
                }
                catch (Exception exception)
                {
                    bcsReport!.ReportFile.StateId = (byte)States.Processing;
                    bcsReport.ReportFile.Info = exception.Message;
                }

                return bcsReport;
            }, cToken)));

        var bcsReportsWithBodies = await Task.WhenAll(bcsrReportsWithHeaders
            .Where(bcsReport => bcsReport.Header is not null)
            .GroupBy(bcsReport => bcsReport.Header!.Agreement)
            .Select(groupingBcsReports => Task.Run(async () =>
            {
                if (!_accountsDictionary.ContainsKey(groupingBcsReports.Key))
                {
                    var reportFileCollection = groupingBcsReports.Select(y => y.ReportFile).ToArray();
                    foreach (var reportFile in reportFileCollection)
                    {
                        reportFile.StateId = (byte)States.Processing;
                        reportFile.Info = $"Account '{groupingBcsReports.Key}' not recognized";
                    }

                    // automatically creating account
                    await _context.Accounts.AddRangeAsync(reportFileCollection
                        .GroupBy(x => x.UserId)
                        .Select(x => new Account
                        {
                            ProviderId = ProviderId,
                            UserId = x.Key,
                            Name = groupingBcsReports.Key
                        }), cToken);

                    return groupingBcsReports;
                }

                var accountId = _accountsDictionary[groupingBcsReports.Key];

                await Task.WhenAll(groupingBcsReports.Select(bcsReport => Task.Run(() =>
                {
                    try
                    {
                        bcsReport.Body = bcsReport.GetBody(accountId);
                    }
                    catch (Exception exception)
                    {
                        bcsReport.ReportFile.StateId = (byte)States.Processing;
                        bcsReport.ReportFile.Info = exception.Message;
                    }

                }, cToken)));

                return groupingBcsReports;
            }, cToken)));

        await SaveAsync(bcsReportsWithBodies.SelectMany(x => x), cToken);
    }
    private async Task SaveAsync(IEnumerable<BcsReport> bcsReports, CancellationToken cToken)
    {
        foreach (var groupingBcsReports in bcsReports.Where(x => x.Header is not null && x.Body is not null).GroupBy(x => x.Body!.AccountId))
        {
            try
            {
                var dateStart = groupingBcsReports.Min(x => x.Header!.DateStart);

                var reports = await _context.Reports
                    .Where(x => x.AccountId == groupingBcsReports.Key && x.DateStart >= dateStart)
                    .ToArrayAsync(cToken);

                var reportAlreadyDates = reports.SelectMany(x =>
                {
                    var date = x.DateStart;
                    var daysCount = x.DateEnd.DayNumber - x.DateStart.DayNumber;
                    var dates = new List<DateOnly>(daysCount) { date };
                    while (daysCount > 0)
                    {
                        date = date.AddDays(1);
                        dates.Add(date);
                        daysCount--;
                    }

                    return dates;
                }).Distinct();

                await _context.Reports.AddRangeAsync(groupingBcsReports.Select(x => new Report
                {
                    ReportFileId = x.ReportFile.Id,
                    AccountId = x.Body!.AccountId,
                    DateStart = x.Header!.DateStart,
                    DateEnd = x.Header.DateEnd
                }), cToken);

                await _context.Deals.AddRangeAsync(groupingBcsReports
                    .SelectMany(x => x.Body!.Deals)
                    .Where(x => !reportAlreadyDates.Contains(x.Date)), cToken);

                await _context.Events.AddRangeAsync(groupingBcsReports
                    .SelectMany(x => x.Body!.Events)
                    .Where(x => !reportAlreadyDates.Contains(x.Date)), cToken);
            }
            catch (Exception exception)
            {
                foreach (var reportFile in groupingBcsReports.Select(y => y.ReportFile))
                {
                    reportFile.StateId = (byte)States.Processing;
                    reportFile.Info = exception.Message;
                }
            }
        }
    }
}