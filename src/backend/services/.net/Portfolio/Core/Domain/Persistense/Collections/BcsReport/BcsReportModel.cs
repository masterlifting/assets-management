using Shared.Persistense.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Collections.BcsReport;

public sealed class BcsReportModel : IPersistensableJson
{
    public string Version { get; init; } = "1.0.0";
    public string Agreement { get; init; } = null!;
    public string DateStart { get; set; } = null!;
    public string DateEnd { get; set; } = null!;
    public IEnumerable<BcsReportEventModel>? Events { get; set; }
    public IEnumerable<BcsReportDealModel>? Deals { get; set; }
    public DateTime Created { get; init; }
    public string? Info { get; set; }
}