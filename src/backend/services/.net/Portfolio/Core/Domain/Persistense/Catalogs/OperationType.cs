using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;

public sealed class OperationType : EntityCatalog
{
    public IEnumerable<EventType>? EventTypes { get; set; }
}