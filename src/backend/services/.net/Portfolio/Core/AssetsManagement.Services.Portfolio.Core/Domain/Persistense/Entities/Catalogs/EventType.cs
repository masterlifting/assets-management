using System.Text.Json.Serialization;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using Shared.Infrastructure.Persistense.Entities.EntityCatalog;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;

public class EventType : Catalog
{
    public virtual OperationType OperationType { get; set; } = null!;
    public int OperationTypeId { get; set; } = (int)Enums.OperationTypes.Default;

    [JsonIgnore]
    public virtual IEnumerable<Event>? Events { get; set; }
}