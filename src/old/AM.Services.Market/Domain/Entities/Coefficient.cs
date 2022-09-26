using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AM.Services.Common.Contracts.Models.Entity.Interfaces;
using AM.Services.Market.Domain.Entities.Catalogs;
using AM.Services.Market.Domain.Entities.Interfaces;
using static AM.Services.Market.Enums;

namespace AM.Services.Market.Domain.Entities;

public class Coefficient : IQuarterIdentity, IRating
{
    public virtual Company Company { get; init; } = null!;
    public string CompanyId { get; set; } = null!;

    public virtual Source Source { get; init; } = null!;
    public byte SourceId { get; set; }

    [Range(1900, 3000)]
    public int Year { get; set; }
    [Range(1, 4)]
    public byte Quarter { get; set; }


    [Column(TypeName = "Decimal(18,4)")]
    public decimal? Pe { get; set; }
    [Column(TypeName = "Decimal(18,4)")]
    public decimal? Pb { get; set; }
    [Column(TypeName = "Decimal(18,4)")]
    public decimal? DebtLoad { get; set; }
    [Column(TypeName = "Decimal(18,4)")]
    public decimal? Profitability { get; set; }
    [Column(TypeName = "Decimal(18,4)")]
    public decimal? Roa { get; set; }
    [Column(TypeName = "Decimal(18,4)")]
    public decimal? Roe { get; set; }
    [Column(TypeName = "Decimal(18,4)")]
    public decimal? Eps { get; set; }


    public virtual Status Status { get; set; } = null!;
    [Range(1, byte.MaxValue)]
    public byte StatusId { get; set; } = (byte)Statuses.New;
    [Column(TypeName = "Decimal(18,4)")]
    public decimal? Result { get; set; }
}