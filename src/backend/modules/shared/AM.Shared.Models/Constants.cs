namespace AM.Shared.Models;

public static class Constants
{
    public static class Assets
    {
        public const string Eur = "Euro";
        public const string Usd = "Dollar of USA";
        public const string Rub = "Russian ruble";
        public const string Gbp = "England Pound sterling";
        public const string Chy = "China Yuan Renminbi";
        public const string Rsd = "Serbian dinar";
        public const string Eth = "Ethereum";
        public const string Btc = "Bitcoin";
    }
    public static class Enums
    {
        public enum AssetTypes
        {
            Things = 1,
            Fiat,
            Stock,
            Bond,
            Fund,
            Crypto,
            NFT,
            Estate
        }
        public enum Zones
        {
            World = 1,
            Eu,
            Rus,
            Usa,
            Chn,
            Gbr,
            Deu,
            Che,
            Jpn,
            Srb,
            Nld,
            Pan,
            Irl,
            Lbr,
            Jey
        }
        public enum Exchanges
        {
            P2P = 1,
            Spbex,
            Moex,
            Nyse,
            Nasdaq,
            Fwb,
            Hkse,
            Lse,
            Sse,
            Binance,
            Ftx2,
            Coinbase,
            Bitokk,
            XChange
        }
    }
}