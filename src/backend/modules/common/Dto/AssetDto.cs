namespace AM.Services.Common.Dto;

public sealed class AssetDto
{
    public string AssetId { get; }
    public int AssetTypeId { get; }
    public int CountryId { get; }
    public string? Name { get; }
    public decimal Balance { get; }
    public decimal? BalanceCost { get; }
    public decimal? LastDealCost { get; }
    public decimal PriceActual { get; }
    public decimal PriceAvg { get; }
    public int Place { get; }

    public AssetDto(string assetId, int assetTypeId, int countryId, string name)
    {
        AssetId = assetId;
        AssetTypeId = assetTypeId;
        CountryId = countryId;
        Name = name;
    }
    public AssetDto(string assetId, int assetTypeId, decimal balance, decimal? balanceCost, decimal? lastDealCost)
    {
        AssetId = assetId;
        AssetTypeId = assetTypeId;
        Balance = balance;
        BalanceCost = balanceCost;
        LastDealCost = lastDealCost;
    }
    public AssetDto(string assetId, int assetTypeId, decimal priceActual, decimal priceAvg)
    {
        AssetId = assetId;
        AssetTypeId = assetTypeId;
        PriceActual = priceActual;
        PriceAvg = priceAvg;
    }
    public AssetDto(string assetId, int assetTypeId, int place)
    {
        AssetId = assetId;
        AssetTypeId = assetTypeId;
        Place = place;
    }
}