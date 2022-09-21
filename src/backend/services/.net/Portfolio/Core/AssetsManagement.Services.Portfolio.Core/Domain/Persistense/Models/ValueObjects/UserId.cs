using AM.Services.Portfolio.Core.Exceptions;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects
{
    public sealed record UserId
    {
        public string AsString { get; }

        public UserId(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 40)
                throw new PortfolioCoreException("", "", "Не удалось определить идентификатор пользователя");

            AsString = value.ToUpper();
        }
    }
}