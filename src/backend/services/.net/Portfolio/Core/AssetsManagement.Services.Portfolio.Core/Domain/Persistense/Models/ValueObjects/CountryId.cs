using AM.Services.Common.Contracts.Entities.Enums;
using AM.Services.Portfolio.Core.Exceptions;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public record CountryId
{
    public int AsInt { get; }
    public Countries AsEnum { get; }

    public CountryId(int value)
    {
        if (!Enum.TryParse<Countries>(value.ToString(), true, out var enumResult))
            throw new PortfolioCoreException("", "", "Не удалось определить страну");

        AsInt = value;
        AsEnum = enumResult;
    }
}