using AM.Services.Portfolio.Core.Exceptions;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;
using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public sealed record EventTypeId
{
    public int AsInt { get; }
    public EventTypes AsEnum { get; }
    public string AsString { get; }

    public EventTypeId(int value)
    {
        if (!Enum.TryParse<EventTypes>(value.ToString(), true, out var enumResult))
            throw new PortfolioCoreException(nameof(EventTypeId), Actions.ValueObject.Set, Actions.ValueObject.ValueNotValidError(value));

        AsInt = value;
        AsEnum = enumResult;
        AsString = enumResult.ToString().ToUpperInvariant();
    }
    public EventTypeId(EventTypes value)
    {
        AsInt = (int)value;
        AsEnum = value;
        AsString = value.ToString().ToUpperInvariant();
    }
}