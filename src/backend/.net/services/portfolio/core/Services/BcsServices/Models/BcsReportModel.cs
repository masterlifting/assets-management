namespace AM.Services.Portfolio.Core.Services.BcsServices.Models;

public sealed class BcsReportModel
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