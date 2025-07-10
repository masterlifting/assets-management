using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Abstractions.Entities.Catalogs;

namespace AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

public sealed class OperationType : PersistentCatalog, IPersistentSql
{
    public IEnumerable<EventType>? EventTypes { get; set; }
}