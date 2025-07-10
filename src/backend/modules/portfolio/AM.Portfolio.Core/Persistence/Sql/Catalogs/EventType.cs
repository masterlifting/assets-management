using Net.Shared.Persistence.Abstractions.Entities;
using Net.Shared.Persistence.Abstractions.Entities.Catalogs;
using Net.Shared.Persistence.Models.Entities.Catalogs;

namespace AM.Portfolio.Core.Persistence.Entities.Sql.Catalogs;

public sealed class EventType : PersistentCatalog, IPersistentSql, IPersistentCatalog
{
    public bool IsIncreasable { get; set; }
    public IEnumerable<Event>? Events { get; set; }
}
