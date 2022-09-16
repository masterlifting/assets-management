using Shared.Exceptions;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public record DerivativeId
{
    public string AsString { get; }

    public DerivativeId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 20)
            throw new SharedCastException("", "", $"Не удалось установить идентификатор дериватива по значению: {value}");

        AsString = value.ToUpper();
    }
}