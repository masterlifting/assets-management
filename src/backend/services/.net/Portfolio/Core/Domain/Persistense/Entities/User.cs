using Shared.Persistense.Abstractions.Entities;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities;

public sealed class User : IEntity
{
    [Key, StringLength(40)]
    public Guid Id { get; init; }

    [Required, StringLength(500)]
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public IEnumerable<Account>? Accounts { get; set; } = null!;
    [JsonIgnore]
    public IEnumerable<Deal>? Deals { get; set; }
    [JsonIgnore]
    public IEnumerable<Event>? Events { get; set; }
    public DateTime Created { get; init; }
    public string? Info { get; set; }
}