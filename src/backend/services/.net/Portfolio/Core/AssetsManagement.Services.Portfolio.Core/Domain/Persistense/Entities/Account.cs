using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

using Shared.Persistense.Abstractions.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities;

public sealed class Account : IEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, StringLength(200, MinimumLength = 3)]
    public string Name { get; init; } = null!;

    public User User { get; init; } = null!;
    public string UserId { get; init; } = null!;

    public Provider Provider { get; init; } = null!;
    public int ProviderId { get; init; }

    public DateOnly DateCreate { get; init; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [JsonIgnore]
    public IEnumerable<Report>? Reports { get; set; }
    [JsonIgnore]
    public IEnumerable<Deal>? Deals { get; set; }
    [JsonIgnore]
    public IEnumerable<Event>? Events { get; set; }

    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
    public string? Info { get; set; }
}