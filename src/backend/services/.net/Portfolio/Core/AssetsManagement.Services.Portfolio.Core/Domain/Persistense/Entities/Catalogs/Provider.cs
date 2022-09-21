using System.Text.Json.Serialization;

using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

using Shared.Persistense.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs
{
    public sealed class Provider : Catalog
    {
        [JsonIgnore]
        public IEnumerable<Deal>? Deals { get; set; }
        [JsonIgnore]
        public IEnumerable<Event>? Events { get; set; }
        [JsonIgnore]
        public IEnumerable<Account>? Accounts { get; set; }
        [JsonIgnore]
        public IEnumerable<Report>? ReportFiles { get; set; }
    }
}