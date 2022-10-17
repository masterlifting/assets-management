using Shared.Persistense.Exceptions;

using static Shared.Persistense.Constants;
using static Shared.Persistense.Constants.Enums;

namespace Shared.Persistense.Models.ValueObject.EntityState;

public sealed record StepId
{
    public int AsInt { get; }
    public Steps AsEnum { get; }
    public string AsString { get; }

    public StepId(int value)
    {
        if (!Enum.TryParse<Steps>(value.ToString(), true, out var enumResult))
            throw new SharedPersistenseEntityStepException(nameof(StepId), Actions.ValueObject.Set, Actions.ValueObject.ValueNotValid(value));

        AsInt = value;
        AsEnum = enumResult;
        AsString = enumResult.ToString().ToUpperInvariant();
    }
    public StepId(Steps value)
    {
        AsInt = (int)value;
        AsEnum = value;
        AsString = value.ToString().ToUpperInvariant();
    }
}