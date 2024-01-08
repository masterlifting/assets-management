namespace AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs.Transactions;

public sealed record BcsTransactionsResult
{
    private DateTime _dateStart;
    private DateTime _dateEnd;

    public string Source { get; init; } = null!;
    public string Agreement { get; init; } = null!;

    public DateTime DateStart { get => _dateStart; init => _dateStart = value.ToUniversalTime(); }
    public DateTime DateEnd { get => _dateEnd; init => _dateEnd = value.ToUniversalTime(); }

    public ICollection<BcsEvent> Events { get; init; } = Array.Empty<BcsEvent>();
    public ICollection<BcsDeal> Deals { get; init; } = Array.Empty<BcsDeal>();
}
