using AM.Services.Portfolio.Core.Exceptions;

namespace AM.Services.Portfolio.Core.Domain.EntityValueObjects;

using static Shared.Persistense.Constants;

public sealed record DerivativeId
{
    public string AsString { get; }

    public DerivativeId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 20)
            throw new PortfolioCoreException(nameof(DerivativeId), Actions.ValueObject.Validate, new(Actions.ValueObject.ValueNotValidError(value)));

        AsString = value.ToUpper();
    }
}