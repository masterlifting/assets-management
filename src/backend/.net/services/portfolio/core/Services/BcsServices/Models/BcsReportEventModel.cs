namespace AM.Services.Portfolio.Core.Services.BcsServices.Models;

public sealed record BcsReportEventModel
{
    public string Asset { get; init; } = null!;
    public decimal Value { get; init; }
    public string Date { get; init; } = null!;
    public string EventType { get; init; } = null!;
    public string Exchange { get; init; } = null!;
    public string? Info { get; init; } = null!;
}