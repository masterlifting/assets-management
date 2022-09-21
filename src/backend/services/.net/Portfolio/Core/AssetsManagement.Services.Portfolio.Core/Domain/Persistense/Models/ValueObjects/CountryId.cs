using AM.Services.Portfolio.Core.Exceptions;

using static AM.Services.Common.Contracts.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects
{
    public sealed record CountryId
    {
        public int AsInt { get; }
        public Countries AsEnum { get; }
        public string AsString { get; }

        public CountryId(int value)
        {
            if (!Enum.TryParse<Countries>(value.ToString(), true, out var enumResult))
                throw new PortfolioCoreException("", "", "Не удалось определить страну");

            AsInt = value;
            AsEnum = enumResult;
            AsString = enumResult.ToString().ToUpper();
        }
    }
}