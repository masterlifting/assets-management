﻿using AM.Services.Portfolio.Core.Exceptions;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;
using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public sealed record OperationTypeId
{
    public int AsInt { get; }
    public OperationTypes AsEnum { get; }
    public string AsString { get; }

    public OperationTypeId(int value)
    {
        if (!Enum.TryParse<OperationTypes>(value.ToString(), true, out var enumResult))
            throw new PortfolioCoreException(nameof(EventTypeId), Actions.ValueObject.Set, Actions.ValueObject.ValueNotValid(value));

        AsInt = value;
        AsEnum = enumResult;
        AsString = enumResult.ToString().ToUpperInvariant();
    }
    public OperationTypeId(OperationTypes value)
    {
        AsInt = (int)value;
        AsEnum = value;
        AsString = value.ToString().ToUpperInvariant();
    }
}