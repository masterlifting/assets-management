using AM.Services.Portfolio.Core.Exceptions;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;
using static Shared.Persistense.Constants;

public sealed record DerivativeCode
{
    public string AsString { get; }

    public DerivativeCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
            throw new PortfolioCoreException(nameof(DerivativeCode), Actions.ValueObject.Set, new(Actions.ValueObject.ValueNotValidError(value)));

        AsString = value.ToUpper();
    }
}