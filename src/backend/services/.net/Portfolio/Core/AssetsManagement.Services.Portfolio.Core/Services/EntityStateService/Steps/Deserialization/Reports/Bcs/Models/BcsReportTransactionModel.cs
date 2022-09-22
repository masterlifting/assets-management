namespace AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Deserialization.Reports.Bcs.Models;

public sealed class BcsReportTransactionModel
{
    public string Info { get; set; } = null!;
    public string Date { get; set; } = null!;
    public string Exchange { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Sum { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public string EventType { get; set; } = null!;
}