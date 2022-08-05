namespace AM.Services.Common.Contracts;

public static class Enums
{
    public enum AssetTypes : byte
    {
        Default = 255,
        Valuable = 1,
        CurrencyFiat = 2,
        Stock = 3,
        Bond = 4,
        Fund = 5,
        CurrencyToken = 6,
        NftToken = 7,
        RealEstate = 8,
        PersonalEstate = 9
    }
    public enum Exchanges : byte
    {
        Default = 255,
        SPBEX = 1,
        MOEX = 2,
        NYSE = 3,
        NASDAQ = 4,
        FWB = 5,
        HKSE = 6,
        LSE = 7,
        SSE = 8,
        Binance = 9,
        FTX2 = 10,
        Coinbase = 11
    }
    public enum Currencies : byte
    {
        Default = 255,
        RUB = 1,
        USD = 2,
        EUR = 3,
        GBP = 4,
        CHY = 5,
    }
    public enum Countries : byte
    {
        Default = 255,
        RUS = 1,
        USA = 2,
        CHN = 3,
        GBR = 4,
        DEU = 5,
        CHE = 6,
        JPN = 7
    }
    public enum RepositoryActions
    {
        Create,
        Update,
        Delete
    }
    public enum CompareType : byte
    {
        Equal = 1,
        More = 2
    }
}
