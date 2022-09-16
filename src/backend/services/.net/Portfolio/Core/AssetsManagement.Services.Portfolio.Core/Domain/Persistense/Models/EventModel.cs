using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models;

public record EventModel
{
    public EntityStateId EntityStateId { get; init; } = null!;

    public decimal Value { get; init; }

    public DateOnly Date { get; init; }

    public EventTypeId EventTypeId { get; init; } = null!;

    public DerivativeId DerivativeId { get; init; } = null!;
    public DerivativeCode DerivativeCode { get; init; } = null!;

    public int AccountId { get; init; }

    public UserId UserId { get; init; } = null!;

    public ProviderId ProviderId { get; init; } = null!;

    public ExchangeId ExchangeId { get; init; } = null!;

    public StateId StateId { get; init; } = null!;
    public StepId StepId { get; init; } = null!;
    public byte Attempt { get; init; }
    public string? Info { get; init; }

    public Event GetEntity() => new()
    {
        Id = EntityStateId.AsString,
        
        Value = Value,
        
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