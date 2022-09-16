using System.Text.Json.Serialization;
using Shared.Infrastructure.Persistense.Entities.EntityCatalog;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;

public class OperationType : Catalog
{
    [JsonIgnore]
    public virtual IEnumerable<EventType>? EventTypes { get; set; }
}