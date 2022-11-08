namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData.Bcs.Models;

public sealed class BcsReportStockActionModel
{
    public string Ticker { get; set; } = null!;
    public string Date { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Info { get; set; } = null!;
    public string Exchange { get; set; } = null!;
    public string EventType { get; set; } = null!;
}