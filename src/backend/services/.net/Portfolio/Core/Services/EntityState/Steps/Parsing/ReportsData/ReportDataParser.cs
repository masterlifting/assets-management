using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData.Bcs;
using Microsoft.Extensions.Logging;
using Shared.Persistense.Abstractions.Handling.EntityState;
using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData;

public sealed class ReportDataParser : IEntityStepHandler<ReportData>
{
    private readonly ILogger _logger;
    public ReportDataParser(ILogger logger) => _logger = logger;

    public async Task HandleAsync(IEnumerable<ReportData> entities, CancellationToken cToken)
    {
        foreach (var filesGroup in entities.GroupBy(x => x.ProviderId))
            switch (filesGroup.Key)
            {
                case (int)Providers.Bcs:
                    {
                        var parser = new BcsReportDataParser(_logger);
                        await parser.StartAsync(filesGroup, cToken);
                    }
                    break;
            }
    }
}