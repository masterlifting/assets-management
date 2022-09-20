using AM.Services.Portfolio.Core.Abstractions.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Operations;

using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Entities.EntityState;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

public class Derivative : IEntityState, IBalance
{
    [StringLength(20, MinimumLength = 1)]
    public string Id { get; init; } = null!;
    [StringLength(50, MinimumLength = 1)]
    public string Code { get; init; } = null!;

    public virtual Asset Asset { get; set; } = null!;
    public string AssetId { get; init; } = null!;
    public int AssetTypeId { get; init; }

    [Column(TypeName = "Decimal(18,10)")]
    public decimal Balance { get; set; }
    [Column(TypeName = "Decimal(18,10)")]
    public decimal BalanceCost { get; set; }
    [Column(TypeName = "Decimal(18,10)")]
    public decimal LastDealCost { get; set; }

    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
    public int StateId { get; set; }
    public virtual State State { get; set; } = null!;
    public int StepId { get; set; }
    public virtual Step Step { get; set; } = null!;
    public byte Attempt { get; set; }
    public string? Info { get; set; }

    [JsonIgnore]
    public virtual IEnumerable<Income>? Incomes { get; set; }
    [JsonIgnore]
    public virtual IEnumerable<Expense>? Expenses { get; set; }
    [JsonIgnore]
    public virtual IEnumerable<Event>? Events { get; set; }
}