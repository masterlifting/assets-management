﻿using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Enums;
using AM.Services.Portfolio.Core.Exceptions;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public record EventTypeId
{
    public int AsInt { get; }
    public EventTypes AsEnum { get; }

    public EventTypeId(int value)
    {
        if (!Enum.TryParse<EventTypes>(value.ToString(), true, out var enumResult))
            throw new PortfolioCoreException("", "", "Не удалось определить тип актива");

        AsInt = value;
        AsEnum = enumResult;
    }
    public EventTypeId(EventTypes value)
    {
        AsInt = (int)value;
        AsEnum = value;
    }
}