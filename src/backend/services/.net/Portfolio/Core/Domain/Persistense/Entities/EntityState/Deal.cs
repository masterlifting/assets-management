﻿using AM.Services.Common.Contracts.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;

using Shared.Persistense.Abstractions.Entities.EntityState;

using System.ComponentModel.DataAnnotations.Schema;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

public sealed class Deal : SharedEntityState
{
    public DateOnly Date { get; init; }

    [Column(TypeName = "Decimal(18,10)")]
    public decimal Cost { get; init; }

    public Income Income { get; init; } = null!;
    public Expense Expense { get; init; } = null!;

    public Account Account { get; set; } = null!;
    public int AccountId { get; set; }

    public User User { get; set; } = null!;
    public string UserId { get; set; } = null!;

    public Provider Provider { get; set; } = null!;
    public int ProviderId { get; set; }

    public Exchange Exchange { get; set; } = null!;
    public int ExchangeId { get; set; }
}