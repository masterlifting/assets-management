using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AM.Services.Common.Contracts.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Interfaces.Persistense.Entities;
using Shared.Infrastructure.Persistense.Entities.EntityState;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

public class Asset : IAsset, IEntityState, IBalance
{
    public string Id { get; init; } = null!;
    public virtual AssetType AssetType { get; set; } = null!;
    public int AssetTypeId { get; init; }

    public virtual Country Country { get; set; } = null!;
    public int CountryId { get; set; }

    [Column(TypeName = "Decimal(18,10)")]
    public decimal Balance { get; set; }
    [Column(TypeName = "Decimal(18,10)")]
    public decimal BalanceCost { get; set; }
    [Column(TypeName = "Decimal(18,10)")]
    public decimal LastDealCost { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
    public int StateId { get; set; }
    public virtual State State { get; set; } = null!;
    public int StepId { get; set; }
    public virtual Step Step { get; set; } = null!;
    public byte Attempt { get; set; }
    public string? Info { get; set; }


    [JsonIgnore]
    public virtual IEnumerable<Derivative>? Derivatives { get; set; }
}