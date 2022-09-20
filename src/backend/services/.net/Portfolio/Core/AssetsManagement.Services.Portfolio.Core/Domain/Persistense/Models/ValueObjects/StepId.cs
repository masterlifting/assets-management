using AM.Services.Portfolio.Core.Exceptions;

using static Shared.Persistense.Constants.Enums;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public sealed record StepId
{
    public int AsInt { get; }
    public Steps AsEnum { get; }

    public StepId(int value)
    {
        if (!Enum.TryParse<Steps>(value.ToString(), true, out var enumResult))
            throw new PortfolioCoreException("", "", "Не удалось определить тип актива");

        AsInt = value;
        AsEnum = enumResult;
    }
}