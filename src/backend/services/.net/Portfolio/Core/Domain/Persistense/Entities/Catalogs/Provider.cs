using System.Text.Json.Serialization;

using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

using Shared.Persistense.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;

public sealed class Provider : Catalog
{
    public Provider()
    {
    }
    public Provider(ProviderId providerId) : base (providerId.AsInt, providerId.AsString)
    {
    }
    [JsonIgnore]
    public IEnumerable<Deal>? Deals { get; set; }
    [JsonIgnore]
    public IEnumerable<Event>? Events { get; set; }
    [JsonIgnore]
    public IEnumerable<Account>? Accounts { get; set; }
    [JsonIgnore]
    public IEnumerable<Report>? ReportFiles { get; set; }
}