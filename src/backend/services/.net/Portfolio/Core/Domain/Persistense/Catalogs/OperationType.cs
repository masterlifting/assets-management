using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;

public sealed class OperationType : Catalog
{
    public IEnumerable<EventType>? EventTypes { get; set; }
}