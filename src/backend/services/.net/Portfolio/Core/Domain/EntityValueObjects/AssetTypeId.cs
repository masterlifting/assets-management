using AM.Services.Portfolio.Core.Exceptions;

using static AM.Services.Common.Contracts.Constants.Persistense.Enums;
using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Domain.EntityValueObjects;

public sealed record AssetTypeId
{
    public int AsInt { get; }
    public AssetTypes AsEnum { get; }

    public AssetTypeId(int value)
    {
        if (!Enum.TryParse<AssetTypes>(value.ToString(), true, out var enumResult))
            throw new PortfolioCoreException(nameof(AssetTypeId), Actions.ValueObject.Validate, new(Actions.ValueObject.ValueNotValidError(value)));

        AsInt = value;
        AsEnum = enumResult;
    }
}