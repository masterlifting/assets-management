using System.Collections.Generic;
using System.Text.Json.Serialization;
using AM.Services.Common.Contracts.Models.Entity;
using AM.Services.Portfolio.Domain.Entities.Catalogs;

namespace AM.Services.Portfolio.Domain.Entities;

public class Asset : Asset<Asset, AssetType, Country>
{
    [JsonIgnore]
    public virtual IEnumerable<Derivative>? Derivatives { get; set; }
}