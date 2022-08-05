using System.Collections.Generic;
using System.Text.Json.Serialization;
using AM.Services.Common.Contracts.Models.Entity;

namespace AM.Services.Portfolio.Domain.Entities.Catalogs;

public class Exchange : Catalog
{
    [JsonIgnore]
    public virtual IEnumerable<Deal>? Deals { get; set; }
    [JsonIgnore]
    public virtual IEnumerable<Event>? Events { get; set; }
}