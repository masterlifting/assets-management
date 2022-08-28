using AM.Services.Portfolio.Domain.Entities;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AM.Services.Portfolio.Services.ReportService;

public interface IReportParserService
{
    Task ParseAsync(IEnumerable<ReportFile> reportFiles, CancellationToken cToken);
}