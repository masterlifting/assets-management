using Shared.Exceptions;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public record DerivativeCode
{
    public string AsString { get; }

    public DerivativeCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
            throw new SharedCastException("", "", $"Не удалось установить код дериватива по значению: {value}");

        AsString = value.ToUpper();
    }
}