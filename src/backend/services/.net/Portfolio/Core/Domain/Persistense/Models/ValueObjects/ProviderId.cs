using AM.Services.Portfolio.Core.Exceptions;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;
using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public sealed record ProviderId
{
    public int AsInt { get; }
    public Providers AsEnum { get; }
    public string AsString { get; }

    public ProviderId(int value)
    {
        if (!Enum.TryParse<Providers>(value.ToString(), true, out var enumResult))
            throw new PortfolioCoreException(nameof(ProviderId), Actions.ValueObject.Set, Actions.ValueObject.ValueNotValidError(value));

        AsInt = value;
        AsEnum = enumResult;
        AsString = enumResult.ToString().ToUpperInvariant();
    }
    public ProviderId(Providers value)
    {
        AsInt = (int)value;
        AsEnum = value;
        AsString = value.ToString().ToUpperInvariant();
    }
}