using AM.Services.Portfolio.Core.Exceptions;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;
using static Shared.Persistense.Constants;

public sealed record DerivativeId
{
    public string AsString { get; }

    public DerivativeId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 20)
            throw new PortfolioCoreException(nameof(DerivativeId), Actions.ValueObject.Set, Actions.ValueObject.ValueNotValidError(value));

        AsString = value.ToUpper();
    }
}