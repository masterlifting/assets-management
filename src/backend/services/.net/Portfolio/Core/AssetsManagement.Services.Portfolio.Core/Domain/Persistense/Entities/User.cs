using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

using Shared.Persistense.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities;

public sealed class User : IEntity
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

    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
    public string? Info { get; set; }
}