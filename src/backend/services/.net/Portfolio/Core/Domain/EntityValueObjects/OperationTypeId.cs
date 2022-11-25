using AM.Services.Portfolio.Core.Exceptions;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;
using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Domain.EntityValueObjects;

public sealed record OperationTypeId
{
    public int AsInt { get; }
    public OperationTypes AsEnum { get; }
    public string AsString { get; }

    public OperationTypeId(int value)
    {
        if (!Enum.TryParse<OperationTypes>(value.ToString(), true, out var enumResult))
            throw new PortfolioCoreException(nameof(EventTypeId), Actions.ValueObject.Validate, new(Actions.ValueObject.ValueNotValidError(value)));

        AsInt = value;
        AsEnum = enumResult;
        AsString = enumResult.ToString().ToUpperInvariant();
    }
    public OperationTypeId(OperationTypes value)
    {
        AsInt = (int)value;
        AsEnum = value;
        AsString = value.ToString().ToUpperInvariant();
    }
}