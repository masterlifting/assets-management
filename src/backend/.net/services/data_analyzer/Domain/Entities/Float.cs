using AM.Services.Common.Contracts.Attributes;
using AM.Services.Common.Contracts.Models.Entity.Interfaces;
using AM.Services.Market.Domain.Entities.Interfaces;

namespace AM.Services.Market.Domain.Entities;

public class Float : IDateIdentity, IDataIdentity
{
    public virtual Company Company { get; init; } = null!;
    public string CompanyId { get; set; } = null!;

    public virtual Source Source { get; init; } = null!;
    public byte SourceId { get; set; }

    public DateOnly Date { get; set; }

    [MoreZero(nameof(Value))]
    public long Value { get; set; }
    public long? ValueFree { get; set; }
}