using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;

public sealed class EventType : EntityCatalog
{
    public OperationType OperationType { get; set; } = null!;
    public int OperationTypeId { get; set; }

    public IEnumerable<Event>? Events { get; set; }
}