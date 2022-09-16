using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using Shared.Infrastructure.Persistense.Entities.EntityCatalog;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;

public class Country : Catalog
{
    public virtual IEnumerable<Asset>? Assets { get; set; }
}