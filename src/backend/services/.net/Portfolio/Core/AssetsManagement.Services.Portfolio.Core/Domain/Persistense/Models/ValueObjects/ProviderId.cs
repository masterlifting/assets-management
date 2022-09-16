﻿using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Enums;

using Shared.Exceptions;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public record ProviderId
{
    public int AsInt { get; }
    public Providers AsEnum { get; }

    public ProviderId(int value)
    {
        if (!Enum.TryParse<Providers>(value.ToString(), true, out var enumResult))
            throw new SharedCastException("", "", "Не удалось определить состояние объекта");

        AsInt = value;
        AsEnum = enumResult;
    }
    public ProviderId(Providers value)
    {
        AsInt = (int)value;
        AsEnum = value;
    }
}