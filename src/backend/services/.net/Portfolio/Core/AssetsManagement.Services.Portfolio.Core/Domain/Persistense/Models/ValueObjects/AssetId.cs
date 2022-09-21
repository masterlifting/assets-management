using AM.Services.Portfolio.Core.Exceptions;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects
{
    public sealed record AssetId
    {
        public string AsString { get; }

        public AssetId(string value)
        {
            if (value.Length > 5)
                throw new PortfolioCoreException("", "", $"Не удалось установить идентификатор активу по значению: {value}");

            AsString = value.ToUpper();
        }
    }
}