using AM.Services.Common.Contracts.Dto;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models;

public sealed record AssetModel
{
    public AssetModel(AssetId assetId, AssetTypeId assetTypeId, CountryId countryId, string? name, string? description)
    {
        AssetId = assetId;
        AssetTypeId = assetTypeId;
        CountryId = countryId;
        Name = name ?? throw new ArgumentNullException(name);
        Description = description;
    }
    public AssetId AssetId { get; } = null!;
    public AssetTypeId AssetTypeId { get; } = null!;
    public CountryId CountryId { get; } = null!;

    public string Name { get; } = null!;
    public string? Description { get; }

    public Asset GetEntity() => new()
    {
        Id = AssetId.AsString,
        AssetTypeId = AssetTypeId.AsInt,
        CountryId = CountryId.AsInt,

        Name = Name,
        Description = Description,
        UpdateTime = DateTime.UtcNow
    };
    public static Asset GetEntity(AssetDto dto) => GetModel(dto).GetEntity();
    public static AssetModel GetModel(Asset asset) => new(
        new AssetId(asset.Id)
        , new AssetTypeId(asset.AssetTypeId)
        , new CountryId(asset.CountryId)
        , asset.Name
        , asset.Description);
    public static AssetModel GetModel(AssetDto dto) => new(
        new AssetId(dto.AssetId)
        , new AssetTypeId(dto.AssetTypeId)
        , new CountryId(dto.CountryId)
        , dto.Name
        , null);
}