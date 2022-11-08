using AM.Services.Portfolio.Core.Exceptions;

using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public sealed record UserId
{
    public string AsString { get; }

    public UserId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 40)
            throw new PortfolioCoreException(nameof(UserId), Actions.ValueObject.Set, new(Actions.ValueObject.ValueNotValidError(value)));

        AsString = value.ToUpper();
    }
}