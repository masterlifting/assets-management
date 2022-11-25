using AM.Services.Portfolio.Core.Exceptions;

using static AM.Services.Common.Contracts.Constants.Persistense.Enums;
using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Domain.EntityValueObjects;

public sealed record ExchangeId
{
    public int AsInt { get; }
    public string AsString { get; }
    public Exchanges AsEnum { get; }

    public ExchangeId(int value)
    {
        if (!Enum.TryParse<Exchanges>(value.ToString(), true, out var enumResult))
            throw new PortfolioCoreException(nameof(ExchangeId), Actions.ValueObject.Validate, new(Actions.ValueObject.ValueNotValidError(value)));

        AsInt = value;
        AsEnum = enumResult;
        AsString = enumResult.ToString().ToUpperInvariant();
    }
    public ExchangeId(string value)
    {
        if (!Enum.TryParse<Exchanges>(value, true, out var enumResult))
            throw new PortfolioCoreException(nameof(ExchangeId), Actions.ValueObject.Validate, new(Actions.ValueObject.ValueNotValidError(value)));

        AsInt = (int)enumResult;
        AsEnum = enumResult;
        AsString = enumResult.ToString().ToUpperInvariant();
    }
    public ExchangeId(Exchanges value)
    {
        AsInt = (int)value;
        AsEnum = value;
        AsString = value.ToString().ToUpperInvariant();
    }
}