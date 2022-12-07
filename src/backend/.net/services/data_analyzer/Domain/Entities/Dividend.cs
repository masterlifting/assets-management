using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AM.Services.Common.Contracts.Models.Entity.Interfaces;
using AM.Services.Market.Domain.Entities.Catalogs;
using AM.Services.Market.Domain.Entities.Interfaces;
using static AM.Services.Market.Enums;

namespace AM.Services.Market.Domain.Entities;

public class Dividend : IDateIdentity, IRating
{
    public virtual Company Company { get; init; } = null!;
    public string CompanyId { get; set; } = null!;

    public virtual Source Source { get; init; } = null!;
    [Range(1, byte.MaxValue)]
    public byte SourceId { get; set; }
    
    public DateOnly Date { get; set; }


    [Column(TypeName = "Decimal(18,4)")]
    public decimal Value { get; set; }

    public virtual Currency Currency { get; set; } = null!;
    [Range(1, byte.MaxValue)]
    public byte CurrencyId { get; set; }


    public virtual Status Status { get; set; } = null!;
    [Range(1, byte.MaxValue)]
    public byte StatusId { get; set; } = (byte)Statuses.New;
    [Column(TypeName = "Decimal(18,4)")]
    public decimal? Result { get; set; }
}