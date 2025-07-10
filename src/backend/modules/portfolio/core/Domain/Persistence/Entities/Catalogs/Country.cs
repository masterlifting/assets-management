using AM.Services.Common.Abstractions.Persistence.Entities.Catalogs;

using Shared.Persistence.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

public sealed class Country : CountryBase, IPersistentSql
{
    public IEnumerable<Asset>? Assets { get; set; }
}
