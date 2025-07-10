using Net.Shared.Persistence.Abstractions.Entities;
using Net.Shared.Persistence.Abstractions.Entities.Catalogs;
using Net.Shared.Persistence.Models.Entities.Catalogs;

namespace AM.Portfolio.Core.Persistence.Entities.Sql.Catalogs;

public sealed class ProcessStatus : PersistentCatalog, IPersistentSql, IPersistentProcessStatus
{
    public IEnumerable<Asset>? Assets { get; set; }
    public IEnumerable<Derivative>? Derivatives { get; set; }
    public IEnumerable<Deal>? Deals { get; set; }
    public IEnumerable<Event>? Events { get; set; }
}
