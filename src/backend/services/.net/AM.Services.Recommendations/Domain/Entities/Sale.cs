using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AM.Services.Recommendations.Domain.Entities;

public class Sale
{
    [Key]
    public int Id { get; set; }

    public virtual Asset Asset { get; set; } = null!;
    public string AssetId { get; set; } = null!;
    public byte AssetTypeId { get; set; }

    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "Decimal(18,10)")]
    public decimal Balance { get; set; }

    [Column(TypeName = "Decimal(18,2)")]
    public decimal ProfitPlan { get; set; }

    [Column(TypeName = "Decimal(18,2)")]
    public decimal? ProfitFact { get; set; }

    [Column(TypeName = "Decimal(18,5)")]
    public decimal PricePlan { get; set; }

    [Column(TypeName = "Decimal(18,5)")]
    public decimal? PriceFact { get; set; }

    public bool IsReady { get; init; }
}