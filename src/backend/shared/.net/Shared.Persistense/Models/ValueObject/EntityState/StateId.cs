﻿using Shared.Persistense.Exceptions;

using static Shared.Persistense.Constants;
using static Shared.Persistense.Constants.Enums;

namespace Shared.Persistense.Models.ValueObject.EntityState;

public sealed record StateId
{
    public int AsInt { get; }
    public States AsEnum { get; }
    public string AsString { get; }

    public StateId(int value)
    {
        if (!Enum.TryParse<States>(value.ToString(), true, out var enumResult))
            throw new SharedPersistenseEntityStateException(nameof(StateId), Actions.ValueObject.Set, Actions.ValueObject.ValueNotValid(value));

        AsInt = value;
        AsEnum = enumResult;
        AsString = enumResult.ToString().ToUpper();
    }
}