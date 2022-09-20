using System.Text.Json.Serialization;

using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

using Shared.Persistense.Entities;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;

public class EventType : Catalog
{
    public virtual OperationType OperationType { get; set; } = null!;
    public int OperationTypeId { get; set; } = (int)OperationTypes.Default;

    [JsonIgnore]
    public virtual IEnumerable<Event>? Events { get; set; }
}