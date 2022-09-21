using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models
{
    public sealed record DealModel(EntityStateId EntityStateId, IncomeModel Income, ExpenseModel Expense)
    {
        public DateOnly Date { get; init; }
        public decimal Cost { get; init; }

        public int AccountId { get; init; }
        public UserId UserId { get; init; } = null!;
        public ProviderId ProviderId { get; init; } = null!;
        public ExchangeId ExchangeId { get; init; } = null!;

        public StateId StateId { get; init; } = null!;
        public StepId StepId { get; init; } = null!;
        public byte Attempt { get; init; }
        public string? Info { get; init; }

        public Deal GetEntity() => new()
        {
            Id = EntityStateId.AsString,

            Income = Income.GetEntity(),
            Expense = Expense.GetEntity(),
            Cost = Cost,

            AccountId = AccountId,
            UserId = UserId.AsString,
            ExchangeId = ExchangeId.AsInt,
            ProviderId = ProviderId.AsInt,

            StateId = StateId.AsInt,
            StepId = StepId.AsInt,
            Attempt = Attempt,
            Info = Info,
            Date = Date,
            UpdateTime = DateTime.UtcNow
        };
    }
}