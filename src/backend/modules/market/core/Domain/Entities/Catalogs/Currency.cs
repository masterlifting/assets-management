using System.Text.Json.Serialization;
using AM.Services.Common.Contracts.Models.Entity;

namespace AM.Services.Market.Domain.Entities.Catalogs;

public class Currency : Catalog
{
    [JsonIgnore]
    public virtual IEnumerable<Price>? Prices { get; set; }
    [JsonIgnore]
    public virtual IEnumerable<Report>? Reports { get; set; }
    [JsonIgnore]
    public virtual IEnumerable<Dividend>? Dividends { get; set; }
}