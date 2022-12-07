using AM.Services.Common.Abstractions.Persistence.Entities.Catalogs;

namespace AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

public sealed class AssetType : AssetTypeBase
{
    public IEnumerable<Asset>? Assets { get; set; }
}
