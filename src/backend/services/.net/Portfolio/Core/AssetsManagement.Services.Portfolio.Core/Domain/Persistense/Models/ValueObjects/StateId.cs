using AM.Services.Portfolio.Core.Exceptions;

using static Shared.Persistense.Constants.Enums;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public record StateId
{
    public int AsInt { get; }
    public States AsEnum { get; }

    public StateId(int value)
    {
        if (!Enum.TryParse<States>(value.ToString(), true, out var enumResult))
            throw new PortfolioCoreException("", "", "Не удалось определить состояние объекта");

        AsInt = value;
        AsEnum = enumResult;
    }
}