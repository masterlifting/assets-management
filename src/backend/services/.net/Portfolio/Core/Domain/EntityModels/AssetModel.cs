using AM.Services.Common.Contracts.Dto;
using AM.Services.Portfolio.Core.Domain.EntityValueObjects;
using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;

namespace AM.Services.Portfolio.Core.Domain.EntityModels;

public sealed record AssetModel
{
    public AssetModel(AssetId assetId, AssetTypeId assetTypeId, CountryId countryId, string? name, string? info)
    {
        AssetId = assetId;
        AssetTypeId = assetTypeId;
        CountryId = countryId;
        Name = name ?? throw new ArgumentNullException(name);
        Info = info;
    }
    public AssetId AssetId { get; } = null!;
    public AssetTypeId AssetTypeId { get; } = null!;
    public CountryId CountryId { get; } = null!;

    public string Name { get; } = null!;
    public string? Info { get; }

    public Asset GetEntity() => new()
    {
        Id = AssetId.AsString,
        TypeId = AssetTypeId.AsInt,
        CountryId = CountryId.AsInt,

        Name = Name,
        Info = Info,
        Updated = DateTime.UtcNow
    };
    public static Asset GetEntity(AssetDto dto) => GetModel(dto).GetEntity();
    public static AssetModel GetModel(Asset asset) => new(
        new AssetId(asset.Id)
        , new AssetTypeId(asset.TypeId)
        , new CountryId(asset.CountryId)
        , asset.Name
        , asset.Info);
    public static AssetModel GetModel(AssetDto dto) => new(
        new AssetId(dto.AssetId)
        , new AssetTypeId(dto.AssetTypeId)
        , new CountryId(dto.CountryId)
        , dto.Name
        , null);
}