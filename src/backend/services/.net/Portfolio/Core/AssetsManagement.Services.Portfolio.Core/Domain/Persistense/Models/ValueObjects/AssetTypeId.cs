using AM.Services.Common.Contracts.Entities.Enums;
using Shared.Exceptions;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public record AssetTypeId
{
    public int AsInt { get; }
    public AssetTypes AsEnum { get; }

    public AssetTypeId(int value)
    {
        if (!Enum.TryParse<AssetTypes>(value.ToString(), true, out var enumResult))
            throw new SharedCastException("", "", "Не удалось определить тип актива");

        AsInt = value;
        AsEnum = enumResult;
    }
}