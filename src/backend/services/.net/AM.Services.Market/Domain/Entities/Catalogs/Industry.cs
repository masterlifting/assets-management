using System.Text.Json.Serialization;
using AM.Services.Common.Contracts.Models.Entity;

namespace AM.Services.Market.Domain.Entities.Catalogs;

public class Industry : Catalog
{
    [JsonIgnore]
    public virtual IEnumerable<Company>? Companies { get; set; }

    public virtual Sector Sector { get; set; } = null!;
    public byte SectorId { get; set; }
}