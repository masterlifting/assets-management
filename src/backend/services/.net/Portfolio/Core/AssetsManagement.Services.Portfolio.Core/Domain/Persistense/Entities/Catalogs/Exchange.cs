using System.Text.Json.Serialization;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using Shared.Infrastructure.Persistense.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;

public class Exchange : Catalog
{
    [JsonIgnore]
    public virtual IEnumerable<Deal>? Deals { get; set; }
    [JsonIgnore]
    public virtual IEnumerable<Event>? Events { get; set; }
}