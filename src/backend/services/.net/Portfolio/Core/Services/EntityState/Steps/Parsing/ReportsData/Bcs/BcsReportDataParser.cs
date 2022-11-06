using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

using Microsoft.Extensions.Logging;

using Shared.Extensions.Serialization;

using System.Text.Json;

using static Shared.Persistense.Constants.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData.Bcs;

public sealed class BcsReportDataParser
{
    private readonly ILogger _logger;

    public BcsReportDataParser(ILogger logger) => _logger = logger;

    public Task StartAsync(IEnumerable<ReportData> files, CancellationToken cToken) => Task.WhenAll(files
        .Select(file => Task.Run(() =>
        {
            try
            {
                var parser = new BcsReportParser(file.Payload);
                var reportModel = parser.GetReportModel();
                file.Json = JsonDocument.Parse(reportModel.Serialize());
            }
            catch (Exception exception)
            {
                file.StateId = (int)States.Error;
                file.Info = $"File: {file.Name}. " + exception.Message;
            }
        }, cToken)));
}