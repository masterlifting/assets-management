namespace AM.Services.Portfolio.Core.Services.BcsServices.Models;

public sealed class BcsReportModel
{
    public string Agreement { get; init; } = null!;
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public IList<BcsReportEventModel> Events { get; set; } = new List<BcsReportEventModel>(0);
    public IList<BcsReportDealModel> Deals { get; set; } = new List<BcsReportDealModel>(0);
    public DateTime Created { get; init; }
    public string? Info { get; set; }
}