using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Enums;

using Shared.Infrastructure.Persistense.Entities.EntityState;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

public class Event : IEntityState
{
    [Key, StringLength(40)]
    public string Id { get; init; } = null!;

    [Column(TypeName = "Decimal(18,10)")]
    public decimal Value { get; set; }

    public DateOnly Date { get; set; }

    public virtual EventType Type { get; set; } = null!;
    public int EventTypeId { get; set; } = (int)EventTypes.Default;

    public virtual Derivative Derivative { get; set; } = null!;
    public string DerivativeId { get; set; } = null!;
    public string DerivativeCode { get; set; } = null!;

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