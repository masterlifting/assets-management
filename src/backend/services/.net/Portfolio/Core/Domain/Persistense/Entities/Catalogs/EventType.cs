using System.Text.Json.Serialization;

using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

using Shared.Persistense.Entities;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs
{
    public sealed class EventType : Catalog
    {
        public OperationType OperationType { get; set; } = null!;
        public int OperationTypeId { get; set; } = (int)OperationTypes.Default;

        [JsonIgnore]
        public IEnumerable<Event>? Events { get; set; }
    }
}