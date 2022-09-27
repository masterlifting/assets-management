using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Deserialization.Reports.Bcs;

using Shared.Persistense.Abstractions.Handling.EntityState;

namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.Reports;

public sealed class ReportDataParser : IEntityStepHandler<ReportData>
{
    public async Task HandleAsync(IEnumerable<ReportData> entities, CancellationToken cToken)
    {
        foreach (var filesGroup in entities.GroupBy(x => x.ProviderId))
            switch (filesGroup.Key)
            {
                case (int)Constants.Persistense.Enums.Providers.Bcs:
                    {
                        var parser = new BcsReportDataParser();
                        await parser.StartAsync(filesGroup, cToken);
                    }
                    break;
            }
    }
}