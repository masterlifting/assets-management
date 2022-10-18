using AM.Services.Portfolio.Core.Exceptions;

using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public sealed record AssetId
{
    public string AsString { get; }

    public AssetId(string value)
    {
        if (value.Length > 5)
            throw new PortfolioCoreException(nameof(AssetId), Actions.ValueObject.Set, Actions.ValueObject.ValueNotValidError(value));

        AsString = value.ToUpper();
    }
}