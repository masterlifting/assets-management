﻿using AM.Services.Portfolio.Core.Domain.EntityValueObjects;
using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;

using Shared.Persistense.ValueObject.EntityState;

namespace AM.Services.Portfolio.Core.Domain.EntityModels;

public sealed record EventModel
{
    public EventModel(
        decimal value
        , DateOnly date
        , EventTypeId eventTypeId
        , DerivativeId derivativeId
        , DerivativeCode derivativeCode
        , AccountId accountId
        , UserId userId
        , ProviderId providerId
        , ExchangeId exchangeId
        , StateId stateId
        , StepId stepId
        , byte attempt
        , string? info)
    {
        EntityStateId = new EntityStateId(Guid.NewGuid());
        Value = value;
        Date = date;
        EventTypeId = eventTypeId;
        DerivativeId = derivativeId;
        DerivativeCode = derivativeCode;
        AccountId = accountId;
        UserId = userId;
        ProviderId = providerId;
        ExchangeId = exchangeId;
        StateId = stateId;
        StepId = stepId;
        Attempt = attempt;
        Info = info;
    }

    public EntityStateId EntityStateId { get; } = null!;

    public decimal Value { get; }

    public DateOnly Date { get; }

    public EventTypeId EventTypeId { get; } = null!;

    public DerivativeId DerivativeId { get; } = null!;
    public DerivativeCode DerivativeCode { get; } = null!;

    public AccountId AccountId { get; } = null!;

    public UserId UserId { get; } = null!;

    public ProviderId ProviderId { get; } = null!;

    public ExchangeId ExchangeId { get; } = null!;

    public StateId StateId { get; } = null!;
    public StepId StepId { get; } = null!;
    public byte Attempt { get; }
    public string? Info { get; }

    public Event GetEntity() => new()
    {
        Id = EntityStateId.Value,

        Value = Value,

        AccountId = AccountId.AsInt,
        UserId = UserId.AsString,
        ExchangeId = ExchangeId.AsInt,
        ProviderId = ProviderId.AsInt,

        StatusId = StateId.AsInt,
        StepId = StepId.AsInt,
        Attempt = Attempt,
        Info = Info,
        Date = Date,
        Updated = DateTime.UtcNow
    };
}