using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

using Shared.Extensions.Serialization;

using System.Text.Json;

using static Shared.Persistense.Constants.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Deserialization.Reports.Bcs
{
    public sealed class BcsReportDataParser
    {
        public Task StartAsync(IEnumerable<ReportData> files, CancellationToken cToken) => Task.WhenAll(files
            .Select(file => Task.Run(() =>
            {
                try
                {
                    var parser = new BcsReportFileParser(file.Payload);
                    var reportModel = parser.GetReportModel();
                    file.Json = JsonDocument.Parse(reportModel.SerializeToString());
                }
                catch (Exception exception)
                {
                    file.StateId = (int)States.Error;
                    file.Info = exception.Message;
                }
            }, cToken)));
    }
}