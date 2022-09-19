using AM.Services.Portfolio.Core.Exceptions;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public record EntityStateId
{
    public string AsString { get; }

    public EntityStateId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 40)
            throw new PortfolioCoreException("", "", "Не удалось определить идентификатор состояния");

        AsString = value.ToUpper();
    }
    public EntityStateId(Guid value)
    {
        AsString = value.ToString().ToUpper();
    }
}