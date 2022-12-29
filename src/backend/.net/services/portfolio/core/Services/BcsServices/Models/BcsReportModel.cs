namespace AM.Services.Portfolio.Core.Services.BcsServices.Models;

public sealed class BcsReportModel
{
    public string Version { get; init; } = "1.0.0";
    public string Agreement { get; init; } = null!;
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public IEnumerable<BcsReportEventModel>? Events { get; set; }
    public IEnumerable<BcsReportDealModel>? Deals { get; set; }
    public DateTime Created { get; init; }
    public string? Info { get; set; }
}