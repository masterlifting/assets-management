namespace AM.Shared.Models.Dto;

public sealed record Asset
{
    public string AssetId { get; set; } = null!;
    public int AssetTypeId { get; set; }
    public int CountryId { get; set; }
    public string? Name { get; set; }
    public decimal Balance { get; set; }
    public decimal? BalanceCost { get; set; }
    public decimal? LastDealCost { get; set; }
    public decimal PriceActual { get; set; }
    public decimal PriceAvg { get; set; }
    public int Place { get; set; }
}