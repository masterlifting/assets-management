using AM.Services.Common.Abstractions.Persistence.Entities;
using AM.Services.Portfolio.Core.Abstractions.Persistence.Entities;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

using Shared.Persistence.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistence.Entities;

public sealed class Asset : IPersistentSql, IAsset, IPersistentProcess, IBalance
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    public decimal BalanceValue { get; set; }
    public decimal BalanceCost { get; set; }
    public decimal LastDealCost { get; set; }

    public ProcessStep ProcessStep { get; set; } = null!;
    public int ProcessStepId { get; set; }
    public ProcessStatus ProcessStatus { get; set; } = null!;
    public int ProcessStatusId { get; set; }
    public byte ProcessAttempt { get; set; }
    public string? Error { get; set; }

    public DateTime Created { get; init; }
    public DateTime Updated { get; set; }

    public AssetType Type { get; init; } = null!;
    public int TypeId { get; init; }

    public Country Country { get; set; } = null!;
    public int CountryId { get; init; }
    public string? Description { get; init; }
}