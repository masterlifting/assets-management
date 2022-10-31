namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData.Bcs.Models;

public sealed class BcsReportBalanceModel
{
    public string Date { get; set; } = null!;
    public string Exchange { get; set; } = null!;
    public string Sum { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public string EventType { get; set; } = null!;
    public string? Info { get; set; }
}