using AM.Services.Portfolio.Core.Exceptions;

using static AM.Services.Common.Contracts.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects
{
    public sealed record ExchangeId
    {
        public int AsInt { get; }
        public Exchanges AsEnum { get; }

        public ExchangeId(int value)
        {
            if (!Enum.TryParse<Exchanges>(value.ToString(), true, out var enumResult))
                throw new PortfolioCoreException("", "", $"Не удалось установить идентификатор объекту состояния по значению: {value}");

            AsInt = value;
            AsEnum = enumResult;
        }
        public ExchangeId(Exchanges value)
        {
            AsInt = (int)value;
            AsEnum = value;
        }
    }
}