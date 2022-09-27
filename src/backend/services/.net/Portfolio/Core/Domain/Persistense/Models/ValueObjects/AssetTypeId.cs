using AM.Services.Portfolio.Core.Exceptions;

using static AM.Services.Common.Contracts.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects
{
    public sealed record AssetTypeId
    {
        public int AsInt { get; }
        public AssetTypes AsEnum { get; }

        public AssetTypeId(int value)
        {
            if (!Enum.TryParse<AssetTypes>(value.ToString(), true, out var enumResult))
                throw new PortfolioCoreException("", "", "Не удалось определить тип актива");

            AsInt = value;
            AsEnum = enumResult;
        }
    }
}