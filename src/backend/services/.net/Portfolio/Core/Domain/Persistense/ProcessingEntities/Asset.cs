using AM.Services.Common.Contracts.Abstractions.Persistense.Entities;
using AM.Services.Portfolio.Core.Abstractions.Persistense.Entities;

using Shared.Persistense.Abstractions.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;

public sealed class Asset : IAsset, IProcessableEntity, IBalance
{
    public int TypeId { get; init; }
    public int CountryId { get; set; }
    public string Name { get; set; }
    public Guid Id { get; init; }
    public int ProcessStatusId { get; set; }
    public int ProcessStepId { get; set; }
    public byte ProcessAttempt { get; set; }
    public DateTime Updated { get; set; }
    public DateTime Created { get; init; }
    public string? Info { get; set; }
    public decimal BalanceValue { get; set; }
    public decimal BalanceCost { get; set; }
    public decimal LastDealCost { get; set; }
}