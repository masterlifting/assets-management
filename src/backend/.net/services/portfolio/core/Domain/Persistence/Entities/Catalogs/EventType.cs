using Shared.Persistence.Abstractions.Entities.Catalogs;

namespace AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

public sealed class EventType : PersistentCatalog
{
    public OperationType OperationType { get; set; } = null!;
    public int OperationTypeId { get; set; }

    public IEnumerable<Event>? Events { get; set; }
}