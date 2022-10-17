using AM.Services.Portfolio.Core.Abstractions.Persistense.Entities;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

public sealed class Derivative : Balance
{
    [StringLength(50, MinimumLength = 1)]
    public string Code { get; init; } = null!;

    public Asset Asset { get; set; } = null!;
    public string AssetId { get; init; } = null!;
    public int AssetTypeId { get; init; }

    [JsonIgnore]
    public IEnumerable<Income>? Incomes { get; set; }
    [JsonIgnore]
    public IEnumerable<Expense>? Expenses { get; set; }
    [JsonIgnore]
    public IEnumerable<Event>? Events { get; set; }
}