using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AM.Services.Common.Contracts.Models.Entity;
using AM.Services.Recommendations.Domain.Entities.Catalogs;

namespace AM.Services.Recommendations.Domain.Entities;

public class Asset : Asset<Asset, AssetType, Country>
{
    public int? RatingPlace { get; set; }
    [Range(0, int.MaxValue), Column(TypeName = "Decimal(18,10)")]
    public decimal? PortfolioBalance { get; set; }
    [Column(TypeName = "Decimal(18,5)")]
    public decimal? PortfolioBalanceCost { get; set; }
    [Column(TypeName = "Decimal(18,5)")]
    public decimal? PortfolioLastDealPrice { get; set; }
    [Column(TypeName = "Decimal(18,5)")]
    public decimal? MarketPriceActual { get; set; }
    [Column(TypeName = "Decimal(18,5)")]
    public decimal? MarketPriceAvg { get; set; }

    public virtual IEnumerable<Purchase>? Purchases { get; set; }
    public virtual IEnumerable<Sale>? Sales { get; set; }
}