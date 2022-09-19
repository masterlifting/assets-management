using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

using Shared.Persistense.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;

public class AssetType : Catalog
{
    public virtual IEnumerable<Asset>? Assets { get; set; }
}