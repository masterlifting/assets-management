using System.Text.Json.Serialization;
using AM.Services.Common.Contracts.Models.Entity;

namespace AM.Services.Market.Domain.Entities.Catalogs;

public class Country : Catalog
{
    [JsonIgnore]
    public virtual IEnumerable<Company>? Companies { get; set; }
}