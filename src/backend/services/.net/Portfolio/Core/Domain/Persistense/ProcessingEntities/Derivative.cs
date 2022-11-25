using AM.Services.Portfolio.Core.Abstractions.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Shared.Persistense.Abstractions.Entities;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;

public sealed class Derivative : IEntityProcessable, IBalance
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
    public DateTime Created { get; init; }
    public string? Info { get; set; }
    public decimal BalanceValue { get; set; }
    public decimal BalanceCost { get; set; }
    public decimal LastDealCost { get; set; }
    public Guid Id { get; init; }
    public int ProcessStatusId { get; set; }
    public int ProcessStepId { get; set; }
    public byte ProcessAttempt { get; set; }
    public DateTime Updated { get; set; }
}