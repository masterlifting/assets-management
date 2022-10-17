using AM.Services.Common.Contracts.Abstractions.Persistense.Entities;
using AM.Services.Common.Contracts.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Abstractions.Persistense.Entities;

using System.Text.Json.Serialization;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

public sealed class Asset : Balance, IAsset
{
    public AssetType AssetType { get; set; } = null!;
    public int TypeId { get; init; }

    public Country Country { get; set; } = null!;
    public int CountryId { get; set; }

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public IEnumerable<Derivative>? Derivatives { get; set; }
}