using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace AM.Services.Common.Contracts;

public static class Constants
{
    public static class Persistense
    {
        public static class Enums
        {
            public enum AssetTypes
            {
                Default = -1,
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
            public enum Countries
            {
                Default = -1,
                Rus = 1,
                Usa = 2,
                Chn = 3,
                Gbr = 4,
                Deu = 5,
                Che = 6,
                Jpn = 7
            }
            public enum Currencies
            {
                Default = -1,
                Rub = 1,
                Usd = 2,
                Eur = 3,
                Gbp = 4,
                Chy = 5,
            }
            public enum Exchanges
            {
                Default = -1,
                Spbex = 1,
                Moex = 2,
                Nyse = 3,
                Nasdaq = 4,
                Fwb = 5,
                Hkse = 6,
                Lse = 7,
                Sse = 8,
                Binance = 9,
                Ftx2 = 10,
                Coinbase = 11
            }
        }
        public static class Catalogs
        {
            public static readonly PersistensableCatalog[] AssetTypes =
            {
        new() {Id = (int) Enums.AssetTypes.Default, Name = nameof(Enums.AssetTypes.Default), Info = "Not defined" },
        new() {Id = (int) Enums.AssetTypes.Valuable, Name = nameof(Enums.AssetTypes.Valuable), Info = "Valuable" },
        new() {Id = (int) Enums.AssetTypes.Stock, Name = nameof(Enums.AssetTypes.Stock), Info = "Stocks" },
        new() {Id = (int) Enums.AssetTypes.Bond, Name = nameof(Enums.AssetTypes.Bond), Info = "Bonds" },
        new() {Id = (int) Enums.AssetTypes.Fund, Name = nameof(Enums.AssetTypes.Fund), Info = "Founds" },
        new() {Id = (int) Enums.AssetTypes.CurrencyFiat, Name = nameof(Enums.AssetTypes.CurrencyFiat), Info = "Fiat currencies" },
        new() {Id = (int) Enums.AssetTypes.CurrencyToken, Name = nameof(Enums.AssetTypes.CurrencyToken), Info = "Crypto currencies" },
        new() {Id = (int) Enums.AssetTypes.NftToken, Name = nameof(Enums.AssetTypes.NftToken), Info = "NFT tokens"},
        new() {Id = (int) Enums.AssetTypes.RealEstate, Name = nameof(Enums.AssetTypes.RealEstate), Info = "Real estates"},
        new() {Id = (int) Enums.AssetTypes.PersonalEstate, Name = nameof(Enums.AssetTypes.PersonalEstate), Info = "Personal estates"}
    };
            public static readonly PersistensableCatalog[] Exchanges =
            {
        new() {Id = (int) Enums.Exchanges.Default, Name = nameof(Enums.Exchanges.Default) },
        new() {Id = (int) Enums.Exchanges.Nasdaq, Name = nameof(Enums.Exchanges.Nasdaq) },
        new() {Id = (int) Enums.Exchanges.Nyse, Name = nameof(Enums.Exchanges.Nyse)},
        new() {Id = (int) Enums.Exchanges.Fwb, Name = nameof(Enums.Exchanges.Fwb)},
        new() {Id = (int) Enums.Exchanges.Hkse, Name = nameof(Enums.Exchanges.Hkse)},
        new() {Id = (int) Enums.Exchanges.Lse, Name = nameof(Enums.Exchanges.Lse)},
        new() {Id = (int) Enums.Exchanges.Sse, Name = nameof(Enums.Exchanges.Sse)},
        new() {Id = (int) Enums.Exchanges.Spbex, Name = nameof(Enums.Exchanges.Spbex)},
        new() {Id = (int) Enums.Exchanges.Moex, Name = nameof(Enums.Exchanges.Moex)},
        new() {Id = (int) Enums.Exchanges.Binance, Name = nameof(Enums.Exchanges.Binance)},
        new() {Id = (int) Enums.Exchanges.Ftx2, Name = nameof(Enums.Exchanges.Ftx2)},
        new() {Id = (int) Enums.Exchanges.Coinbase, Name = nameof(Enums.Exchanges.Coinbase)}
    };
            public static readonly PersistensableCatalog[] Currencies =
            {
        new() {Id = (int) Enums.Currencies.Default, Name = nameof(Enums.Currencies.Default), Info = "Not defined" },
        new() {Id = (int) Enums.Currencies.Rub, Name = nameof(Enums.Currencies.Rub), Info = "₽"},
        new() {Id = (int) Enums.Currencies.Usd, Name = nameof(Enums.Currencies.Usd), Info = "$"},
        new() {Id = (int) Enums.Currencies.Eur, Name = nameof(Enums.Currencies.Eur), Info = "€"},
        new() {Id = (int) Enums.Currencies.Gbp, Name = nameof(Enums.Currencies.Gbp), Info = "£"},
        new() {Id = (int) Enums.Currencies.Chy, Name = nameof(Enums.Currencies.Chy), Info = "¥"}
    };
            public static readonly PersistensableCatalog[] Countries =
            {
        new() { Id = (int) Enums.Countries.Default, Name = nameof(Enums.Countries.Default), Info = "Not defined" },
        new() { Id = (int) Enums.Countries.Rus, Name = nameof(Enums.Countries.Rus), Info = "Russia" },
        new() { Id = (int) Enums.Countries.Usa, Name = nameof(Enums.Countries.Usa), Info = "USA" },
        new() { Id = (int) Enums.Countries.Chn, Name = nameof(Enums.Countries.Chn), Info = "China" },
        new() { Id = (int) Enums.Countries.Deu, Name = nameof(Enums.Countries.Deu), Info = "Deutschland" },
        new() { Id = (int) Enums.Countries.Gbr, Name = nameof(Enums.Countries.Gbr), Info = "Great Britain" },
        new() { Id = (int) Enums.Countries.Che, Name = nameof(Enums.Countries.Che), Info = "Switzerland" },
        new() { Id = (int) Enums.Countries.Jpn, Name = nameof(Enums.Countries.Jpn), Info = "Japan" }
    };
        }
    }
}