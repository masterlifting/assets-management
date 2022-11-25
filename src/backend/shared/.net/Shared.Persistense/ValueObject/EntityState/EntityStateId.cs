using Shared.Persistense.Exceptions;

namespace Shared.Persistense.ValueObject.EntityState;

using static Shared.Persistense.Constants;

public sealed record EntityStateId
{
    public string Value { get; }

    public EntityStateId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 40)
            throw new SharedPersistenseEntityStateException(nameof(EntityStateId), Actions.ValueObject.Validate, new(Actions.ValueObject.ValueNotValidError(value)));

        Value = value.ToUpper();
    }
    public EntityStateId(Guid value)
    {
        if (value == Guid.Empty)
            throw new SharedPersistenseEntityStateException(nameof(EntityStateId), Actions.ValueObject.Validate, new(Actions.ValueObject.ValueNotValidError(value.ToString())));

        Value = value.ToString().ToUpper();
    }
}