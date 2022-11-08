using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;
using AM.Services.Portfolio.Core.Exceptions;

using Shared.Persistense.Models.ValueObject.EntityState;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models;

public sealed record DealModel
{
    public DealModel(
        decimal cost
        , DateOnly date
        , IncomeModel income
        , ExpenseModel expense
        , AccountId accountId
        , UserId userId
        , ProviderId providerId
        , ExchangeId exchangeId
        , StateId stateId
        , StepId stepId
        , byte attempt
        , string? info)
    {
        EntityStateId = income.DealId.Equals(expense.DealId)
            ? income.DealId.AsEntityStateId
            : throw new PortfolioCoreException(nameof(DealModel), "Receiving DealId", new("DealId from Income and DealId from Expence are not comparision"));
        Income = income;
        Expense = expense;
        Date = date;
        Cost = cost;
        AccountId = accountId;
        UserId = userId;
        ProviderId = providerId;
        ExchangeId = exchangeId;
        StateId = stateId;
        StepId = stepId;
        Attempt = attempt;
        Info = info;
    }

    public EntityStateId EntityStateId { get; }
    public IncomeModel Income { get; }
    public ExpenseModel Expense { get; }
    public DateOnly Date { get; init; }
    public decimal Cost { get; init; }

    public AccountId AccountId { get; init; } = null!;
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

        AccountId = AccountId.AsInt,
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