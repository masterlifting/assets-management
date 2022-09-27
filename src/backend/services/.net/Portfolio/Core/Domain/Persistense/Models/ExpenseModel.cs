using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models
{
    public sealed class ExpenseModel
    {
        public ExpenseModel(EntityStateId dealId, DerivativeId derivativeId, DerivativeCode derivativeCode, decimal value, DateOnly date)
        {
            DerivativeId = derivativeId;
            DerivativeCode = derivativeCode;
            DealId = dealId;
            Value = value;
            Date = date;
        }
        public DerivativeId DerivativeId { get; }
        public DerivativeCode DerivativeCode { get; }
        public EntityStateId DealId { get; }

        public decimal Value { get; }
        public DateOnly Date { get; }

        public Expense GetEntity() => new()
        {
            DerivativeId = DerivativeId.AsString,
            DerivativeCode = DerivativeCode.AsString,
            DealId = DealId.AsString,
            Value = Value,
            Date = Date,
            UpdateTime = DateTime.UtcNow
        };
    }
}