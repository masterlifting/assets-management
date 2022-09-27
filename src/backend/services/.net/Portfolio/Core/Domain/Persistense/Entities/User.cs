using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

using Shared.Persistense.Abstractions.Entities;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities
{
    public sealed class User : SharedEntity
    {
        [Key, StringLength(40)]
        public string Id { get; init; } = null!;

        [Required, StringLength(500)]
        public string Name { get; set; } = null!;

        [JsonIgnore]
        public IEnumerable<Report>? ReportFiles { get; set; }
        [JsonIgnore]
        public IEnumerable<Account>? Accounts { get; set; } = null!;
        [JsonIgnore]
        public IEnumerable<Deal>? Deals { get; set; }
        [JsonIgnore]
        public IEnumerable<Event>? Events { get; set; }
    }
}