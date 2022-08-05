namespace AM.Services.Common.Contracts.Models.Entity;

public static class Catalogs
{
    public static readonly Catalog[] AssetTypes =
    {
        new() {Id = (byte) Enums.AssetTypes.Default, Name = nameof(Enums.AssetTypes.Default), Description = "Не определено" },
        new() {Id = (byte) Enums.AssetTypes.Valuable, Name = nameof(Enums.AssetTypes.Valuable), Description = "Драгоценности" },
        new() {Id = (byte) Enums.AssetTypes.Stock, Name = nameof(Enums.AssetTypes.Stock), Description = "Акции" },
        new() {Id = (byte) Enums.AssetTypes.Bond, Name = nameof(Enums.AssetTypes.Bond), Description = "Облигации" },
        new() {Id = (byte) Enums.AssetTypes.Fund, Name = nameof(Enums.AssetTypes.Fund), Description = "Фонды" },
        new() {Id = (byte) Enums.AssetTypes.CurrencyFiat, Name = nameof(Enums.AssetTypes.CurrencyFiat), Description = "Фиатные валюты" },
        new() {Id = (byte) Enums.AssetTypes.CurrencyToken, Name = nameof(Enums.AssetTypes.CurrencyToken), Description = "Электронные валюты" },
        new() {Id = (byte) Enums.AssetTypes.NftToken, Name = nameof(Enums.AssetTypes.NftToken), Description = "NFT"},
        new() {Id = (byte) Enums.AssetTypes.RealEstate, Name = nameof(Enums.AssetTypes.RealEstate), Description = "Недвижимое имущество"},
        new() {Id = (byte) Enums.AssetTypes.PersonalEstate, Name = nameof(Enums.AssetTypes.PersonalEstate), Description = "Движимое имущество"}
    };

    public static readonly Catalog[] Exchanges =
    {
        new() {Id = (byte) Enums.Exchanges.Default, Name = nameof(Enums.Exchanges.Default) },
        new() {Id = (byte) Enums.Exchanges.NASDAQ, Name = nameof(Enums.Exchanges.NASDAQ) },
        new() {Id = (byte) Enums.Exchanges.NYSE, Name = nameof(Enums.Exchanges.NYSE)},
        new() {Id = (byte) Enums.Exchanges.FWB, Name = nameof(Enums.Exchanges.FWB)},
        new() {Id = (byte) Enums.Exchanges.HKSE, Name = nameof(Enums.Exchanges.HKSE)},
        new() {Id = (byte) Enums.Exchanges.LSE, Name = nameof(Enums.Exchanges.LSE)},
        new() {Id = (byte) Enums.Exchanges.SSE, Name = nameof(Enums.Exchanges.SSE)},
        new() {Id = (byte) Enums.Exchanges.SPBEX, Name = nameof(Enums.Exchanges.SPBEX)},
        new() {Id = (byte) Enums.Exchanges.MOEX, Name = nameof(Enums.Exchanges.MOEX)},
        new() {Id = (byte) Enums.Exchanges.Binance, Name = nameof(Enums.Exchanges.Binance)},
        new() {Id = (byte) Enums.Exchanges.FTX2, Name = nameof(Enums.Exchanges.FTX2)},
        new() {Id = (byte) Enums.Exchanges.Coinbase, Name = nameof(Enums.Exchanges.Coinbase)}
    };

    public static readonly Catalog[] Currencies =
    {
        new() {Id = (byte) Enums.Currencies.Default, Name = nameof(Enums.Currencies.Default), Description = "Не определено" },
        new() {Id = (byte) Enums.Currencies.RUB, Name = nameof(Enums.Currencies.RUB), Description = "₽"},
        new() {Id = (byte) Enums.Currencies.USD, Name = nameof(Enums.Currencies.USD), Description = "$"},
        new() {Id = (byte) Enums.Currencies.EUR, Name = nameof(Enums.Currencies.EUR), Description = "€"},
        new() {Id = (byte) Enums.Currencies.GBP, Name = nameof(Enums.Currencies.GBP), Description = "£"},
        new() {Id = (byte) Enums.Currencies.CHY, Name = nameof(Enums.Currencies.CHY), Description = "¥"}
    };

    public static readonly Catalog[] Countries =
    {
        new() { Id = (byte) Enums.Countries.Default, Name = nameof(Enums.Countries.Default), Description = "Не определено" },
        new() { Id = (byte) Enums.Countries.RUS, Name = nameof(Enums.Countries.RUS), Description = "Russia" },
        new() { Id = (byte) Enums.Countries.USA, Name = nameof(Enums.Countries.USA), Description = "USA" },
        new() { Id = (byte) Enums.Countries.CHN, Name = nameof(Enums.Countries.CHN), Description = "China" },
        new() { Id = (byte) Enums.Countries.DEU, Name = nameof(Enums.Countries.DEU), Description = "Deutschland" },
        new() { Id = (byte) Enums.Countries.GBR, Name = nameof(Enums.Countries.GBR), Description = "Great Britain" },
        new() { Id = (byte) Enums.Countries.CHE, Name = nameof(Enums.Countries.CHE), Description = "Switzerland" },
        new() { Id = (byte) Enums.Countries.JPN, Name = nameof(Enums.Countries.JPN), Description = "Japan" }
    };
}