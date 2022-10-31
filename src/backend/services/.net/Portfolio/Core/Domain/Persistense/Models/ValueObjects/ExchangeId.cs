using AM.Services.Portfolio.Core.Exceptions;

using static AM.Services.Common.Contracts.Constants.Persistense.Enums;
using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public sealed record ExchangeId
{
    public int AsInt { get; }
    public string AsString { get; }
    public Exchanges AsEnum { get; }

    public ExchangeId(int value)
    {
        if (!Enum.TryParse<Exchanges>(value.ToString(), true, out var enumResult))
            throw new PortfolioCoreException(nameof(ExchangeId), Actions.ValueObject.Set, Actions.ValueObject.ValueNotValidError(value));

        AsInt = value;
        AsEnum = enumResult;
        AsString = enumResult.ToString().ToUpperInvariant();
    }
    public ExchangeId(Exchanges value)
    {
        AsInt = (int)value;
        AsEnum = value;
        AsString = value.ToString().ToUpperInvariant();
    }
    public ExchangeId(string value, IDictionary<string, int> exchangeDictionary)
    {
        if (!exchangeDictionary.ContainsKey(value))
            throw new PortfolioCoreException(nameof(ExchangeId), Actions.ValueObject.Set, Actions.ValueObject.ValueNotValidError(value));

        var asInt = exchangeDictionary[value];

        if (asInt <= 0)
            throw new PortfolioCoreException(nameof(ExchangeId), Actions.ValueObject.Set, Actions.ValueObject.ValueNotValidError(value));

        if (!Enum.TryParse<Exchanges>(asInt.ToString(), true, out var enumResult))
            throw new PortfolioCoreException(nameof(ExchangeId), Actions.ValueObject.Set, Actions.ValueObject.ValueNotValidError(value));

        AsInt = asInt;
        AsEnum = enumResult;
        AsString = enumResult.ToString().ToUpperInvariant();
    }
}