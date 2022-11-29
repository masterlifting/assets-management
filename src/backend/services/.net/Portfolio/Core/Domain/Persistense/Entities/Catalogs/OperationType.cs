using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;

public sealed class OperationType : PersistensableCatalog
{
    public IEnumerable<EventType>? EventTypes { get; set; }
}