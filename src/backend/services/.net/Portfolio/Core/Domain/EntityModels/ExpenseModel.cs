using AM.Services.Portfolio.Core.Domain.EntityValueObjects;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

namespace AM.Services.Portfolio.Core.Domain.EntityModels;

public sealed record ExpenseModel
{
    public ExpenseModel(DealId dealId, DerivativeId derivativeId, DerivativeCode derivativeCode, decimal value, DateOnly date)
    {
        DerivativeId = derivativeId;
        DerivativeCode = derivativeCode;
        DealId = dealId;
        Value = value;
        Date = date;
    }
    public DealId DealId { get; }
    public DerivativeId DerivativeId { get; }
    public DerivativeCode DerivativeCode { get; }

    public decimal Value { get; }
    public DateOnly Date { get; }

    public Expense GetEntity() => new()
    {
        DerivativeId = DerivativeId.AsString,
        DerivativeCode = DerivativeCode.AsString,
        DealId = DealId.AsString,
        Value = Value,
        Date = Date,
        Updated = DateTime.UtcNow
    };
}