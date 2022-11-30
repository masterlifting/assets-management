﻿using Shared.Persistence.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistence.Entities;

public sealed class Expense : IPersistent
{
    public long Id { get; init; }
    
    public DateOnly Date { get; set; }
    public decimal Value { get; set; }

    public DateTime Created { get; init; }
    public string? Info { get; set; }

    public Derivative Derivative { get; set; } = null!;
    public Guid DerivativeId { get; set; }

    public Deal Deal { get; set; } = null!;
    public Guid DealId { get; set; }
}