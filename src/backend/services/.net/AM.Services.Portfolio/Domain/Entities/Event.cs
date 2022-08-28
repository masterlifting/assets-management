using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AM.Services.Portfolio.Domain.Entities.Catalogs;
using static AM.Services.Portfolio.Enums;
using static AM.Services.Common.Contracts.Enums;

namespace AM.Services.Portfolio.Domain.Entities;

public class Event
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; init; }

    [Column(TypeName = "Decimal(18,10)")]
    public decimal Value { get; set; }
    
    public DateOnly Date { get; set; }
    
    [Required, StringLength(500)]
    public string Info { get; set; } = null!;

    public virtual EventType Type { get; set; } = null!;
    public byte TypeId { get; set; } = (byte)EventTypes.Default;

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
    public byte ExchangeId { get; set; } = (byte) Exchanges.Default;

    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
}