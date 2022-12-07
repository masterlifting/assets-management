using System.Text.Json.Serialization;
using AM.Services.Common.Contracts.Models.Entity;

namespace AM.Services.Market.Domain.Entities.Catalogs;

public class Sector : Catalog
{
    [JsonIgnore]
    public virtual IEnumerable<Industry>? Industries { get; set; }
}