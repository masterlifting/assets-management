using System.Collections.Generic;
using System.Text.Json.Serialization;
using AM.Services.Common.Contracts.Models.Entity;

namespace AM.Services.Portfolio.Domain.Entities.Catalogs;

public class OperationType : Catalog
{
    [JsonIgnore]
    public virtual IEnumerable<EventType>? EventTypes { get; set; }
}