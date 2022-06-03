﻿using System;
using System.ComponentModel.DataAnnotations;
using IM.Service.Portfolio.Domain.Entities.Catalogs;

namespace IM.Service.Portfolio.Domain.Entities;

public class Event
{
    [Key]
    public long Id { get; init; }
    public decimal Cost { get; set; }
    public string? Info { get; set; }
    public DateOnly Date { get; set; }

    public virtual EventType EventType { get; set; } = null!;
    public byte EventTypeId { get; set; }

    public virtual Derivative? Derivative { get; set; }
    public string? DerivativeId { get; set; }
    public string? DerivativeCode { get; set; }

    public virtual Exchange? Exchange { get; set; }
    public byte? ExchangeId { get; set; }

    public virtual Account Account { get; set; } = null!;
    public int AccountId { get; set; }

    public virtual User User { get; set; } = null!;
    public string UserId { get; set; } = null!;

    public virtual Broker Broker { get; set; } = null!;
    public byte BrokerId { get; set; }

    public virtual Currency Currency { get; set; } = null!;
    public byte CurrencyId { get; set; }

    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
}