using AM.Services.Portfolio.Domain.DataAccess;
using AM.Services.Portfolio.Domain.Entities;
using AM.Services.Portfolio.Services.ReportService.Parsers.Bcs;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static AM.Services.Portfolio.Enums;

namespace AM.Services.Portfolio.Services.ReportService;

public class ReportParserService : IReportParserService
{
    private readonly ILogger _logger;
    private readonly DatabaseContext _context;

    public ReportParserService(ILogger logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task ParseAsync(IEnumerable<ReportFile> reportFiles, CancellationToken cToken)
    {
        var derivatives = await _context.Derivatives
            .Select(x => ValueTuple.Create(x.Id, x.Code))
            .ToArrayAsync(cToken);
        var derivativesDictionary = derivatives
            .GroupBy(x => x.Item1)
            .ToDictionary(x => x.Key, x => x.Select(y => y.Item2).ToArray());

        foreach (var providerFiles in reportFiles.GroupBy(x => x.ProviderId))
        {
            var providerId = providerFiles.Key;
            var accountsDictionary = await _context.Accounts
                .Where(x => x.ProviderId == providerId)
                .Select(x => ValueTuple.Create(x.Name, x.Id))
                .ToDictionaryAsync(x => x.Item1, x => x.Item2, cToken);

            switch (providerId)
            {
                case (int)Providers.BCS:
                    {
                        var service = new BcsReportService(_logger, _context, accountsDictionary, derivativesDictionary);
                        await service.ParseAsync(providerFiles, cToken);
                    }
                    break;
            }
        }
    }
}