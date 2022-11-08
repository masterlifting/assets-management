using Shared.Persistense.Exceptions;

namespace Shared.Persistense.Models.ValueObject.EntityState;
using static Shared.Persistense.Constants;

public sealed record EntityStateId
{
    public string AsString { get; }

    public EntityStateId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 40)
            throw new SharedPersistenseEntityStateException(nameof(EntityStateId), Actions.ValueObject.Set, new(Actions.ValueObject.ValueNotValidError(value)));

        AsString = value.ToUpper();
    }
    public EntityStateId(Guid value)
    {
        AsString = value.ToString().ToUpper();
    }
}