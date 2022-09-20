using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Persistense.Entities.EntityState;
using Shared.Persistense.Abstractions.Entities.EntityState;
using AM.Services.Common.Contracts.Persistense.Entities.Catalogs;
using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

public sealed class Event : IEntityState
{
    [Key, StringLength(40)]
    public string Id { get; init; } = null!;

    [Column(TypeName = "Decimal(18,10)")]
    public decimal Value { get; set; }

    public DateOnly Date { get; set; }

    public EventType Type { get; set; } = null!;
    public int EventTypeId { get; set; } = (int)EventTypes.Default;

    public Derivative Derivative { get; set; } = null!;
    public string DerivativeId { get; set; } = null!;
    public string DerivativeCode { get; set; } = null!;

    public Account Account { get; set; } = null!;
    public int AccountId { get; set; }

    public User User { get; set; } = null!;
    public string UserId { get; set; } = null!;

    public Provider Provider { get; set; } = null!;
    public int ProviderId { get; set; }

    public Exchange Exchange { get; set; } = null!;
    public int ExchangeId { get; set; }

    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
    public int StateId { get; set; }
    public State State { get; set; } = null!;
    public int StepId { get; set; }
    public Step Step { get; set; } = null!;
    public byte Attempt { get; set; }
    public string? Info { get; set; }
}