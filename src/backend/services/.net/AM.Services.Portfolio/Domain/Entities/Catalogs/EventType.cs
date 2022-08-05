using System.Collections.Generic;
using System.Text.Json.Serialization;
using AM.Services.Common.Contracts.Models.Entity;

namespace AM.Services.Portfolio.Domain.Entities.Catalogs;

public class EventType : Catalog
{
    public virtual OperationType OperationType { get; set; } = null!;
    public byte OperationTypeId { get; set; } = (byte)Enums.OperationTypes.Default;

    [JsonIgnore]
    public virtual IEnumerable<Event>? Events { get; set; }
}