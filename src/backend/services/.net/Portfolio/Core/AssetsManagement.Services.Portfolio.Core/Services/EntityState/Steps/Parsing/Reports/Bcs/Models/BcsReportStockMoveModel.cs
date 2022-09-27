namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.Reports.Bcs.Models;

public sealed class BcsReportStockMoveModel
{
    public string Ticker { get; set; } = null!;
    public string Date { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Info { get; set; } = null!;
}