﻿using AM.Services.Portfolio.Core.Abstractions.Persistence.Entities;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

using Shared.Persistence.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistence.Entities;

public sealed class Derivative : IPersistentProcess, IBalance
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;

    public decimal BalanceValue { get; set; }
    public decimal BalanceCost { get; set; }
    public decimal LastDealCost { get; set; }

    public ProcessStep ProcessStep { get; set; } = null!;
    public int ProcessStepId { get; set; }
    public ProcessStatus ProcessStatus { get; set; } = null!;
    public int ProcessStatusId { get; set; }
    public byte ProcessAttempt { get; set; }

    public DateTime Created { get; init; }
    public DateTime Updated { get; set; }
    public string? Info { get; set; }

    public Asset Asset { get; set; } = null!;
    public string AssetId { get; init; } = null!;
    public int AssetTypeId { get; init; }

    public IEnumerable<Income>? Incomes { get; set; }
    public IEnumerable<Expense>? Expenses { get; set; }
    public IEnumerable<Event>? Events { get; set; }
}