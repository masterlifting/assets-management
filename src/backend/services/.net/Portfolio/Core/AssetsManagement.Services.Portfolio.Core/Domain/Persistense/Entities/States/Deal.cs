using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Persistense.Abstractions.Entities.State;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

public class Deal : IEntityState
{
    [Key, StringLength(40)]
    public string Id { get; init; } = null!;
    public DateOnly Date { get; init; }

    [Column(TypeName = "Decimal(18,10)")]
    public decimal Cost { get; init; }

    public virtual Income Income { get; init; } = null!;
    public virtual Expense Expense { get; init; } = null!;

    public virtual Account Account { get; set; } = null!;
    public int AccountId { get; set; }

    public virtual User User { get; set; } = null!;
    public string UserId { get; set; } = null!;

    public virtual Provider Provider { get; set; } = null!;
    public int ProviderId { get; set; }

    public virtual Exchange Exchange { get; set; } = null!;
    public int ExchangeId { get; set; }

    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
    public int StateId { get; set; }
    public virtual State State { get; set; } = null!;
    public int StepId { get; set; }
    public virtual Step Step { get; set; } = null!;
    public byte Attempt { get; set; }
    public string? Info { get; set; }
}