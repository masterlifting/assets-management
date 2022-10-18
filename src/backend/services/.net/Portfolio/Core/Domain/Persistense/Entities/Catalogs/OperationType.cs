using System.Text.Json.Serialization;

using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

using Shared.Persistense.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;

public sealed class OperationType : Catalog
{
    public OperationType()
    {
    }
    public OperationType(OperationTypeId operationTypeId) : base (operationTypeId.AsInt, operationTypeId.AsString)
    {
    }

    [JsonIgnore]
    public IEnumerable<EventType>? EventTypes { get; set; }
}