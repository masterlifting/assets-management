using System.Text.Json.Serialization;

using Shared.Persistense.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;

public sealed class OperationType : Catalog
{
    [JsonIgnore]
    public IEnumerable<EventType>? EventTypes { get; set; }
}