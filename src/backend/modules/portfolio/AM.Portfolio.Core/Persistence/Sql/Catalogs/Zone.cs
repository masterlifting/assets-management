using Net.Shared.Persistence.Abstractions.Entities;
using Net.Shared.Persistence.Abstractions.Entities.Catalogs;
using Net.Shared.Persistence.Models.Entities.Catalogs;

namespace AM.Portfolio.Core.Persistence.Entities.Sql.Catalogs;

public sealed class Zone : PersistentCatalog, IPersistentSql, IPersistentCatalog
{
    public IEnumerable<Derivative>? Derivatives { get; set; }
}
