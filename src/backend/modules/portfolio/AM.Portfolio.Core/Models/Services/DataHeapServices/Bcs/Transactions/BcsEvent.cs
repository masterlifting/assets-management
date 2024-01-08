using static AM.Portfolio.Core.Constants.Enums;
using static AM.Shared.Models.Constants.Enums;

namespace AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs.Transactions;

public sealed record BcsEvent
{
    private DateTime _date;
    public BcsAsset Asset { get; init; } = null!;
    public decimal Value { get; init; }
    public DateTime Date { get => _date; init => _date = value.ToUniversalTime(); }
    public BcsReportEvents Event { get; init; }
    public Exchanges Exchange { get; init; }
    public string? Info { get; init; }
}
