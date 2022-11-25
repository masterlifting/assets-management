namespace AM.Services.Portfolio.Core.Domain.EntityStateModels.Report.Bcs;

public sealed record BcsEventTypeModel
{
    public string Asset { get; init; } = null!;
    public decimal Value { get; init; }
    public string Date { get; init; } = null!;
    public string EventType { get; init; } = null!;
    public string Exchange { get; init; } = null!;
    public string? Info { get; init; } = null!;
}