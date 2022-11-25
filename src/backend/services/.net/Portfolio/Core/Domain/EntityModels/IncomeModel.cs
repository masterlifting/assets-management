using AM.Services.Portfolio.Core.Domain.EntityValueObjects;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

namespace AM.Services.Portfolio.Core.Domain.EntityModels;

public sealed record IncomeModel
{
    public IncomeModel(DealId dealId, DerivativeId derivativeId, DerivativeCode derivativeCode, decimal value, DateOnly date)
    {
        DealId = dealId;
        DerivativeId = derivativeId;
        DerivativeCode = derivativeCode;
        Value = value;
        Date = date;
    }
    public DealId DealId { get; }
    public DerivativeId DerivativeId { get; }
    public DerivativeCode DerivativeCode { get; }

    public decimal Value { get; }
    public DateOnly Date { get; }

    public Income GetEntity() => new()
    {
        DerivativeId = DerivativeId.AsString,
        DerivativeCode = DerivativeCode.AsString,
        DealId = DealId.AsString,
        Value = Value,
        Date = Date,
        Updated = DateTime.UtcNow
    };
}