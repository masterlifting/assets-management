using Shared.Infrastructure.Persistense.Entities;

namespace AM.Services.Common.Contracts.Entities;

public static class Catalogs
{
    public static readonly Catalog[] AssetTypes =
    {
        new() {Id = (int) Enums.AssetTypes.Default, Name = nameof(Enums.AssetTypes.Default), Description = "Не определено" },
        new() {Id = (int) Enums.AssetTypes.Valuable, Name = nameof(Enums.AssetTypes.Valuable), Description = "Драгоценности" },
        new() {Id = (int) Enums.AssetTypes.Stock, Name = nameof(Enums.AssetTypes.Stock), Description = "Акции" },
        new() {Id = (int) Enums.AssetTypes.Bond, Name = nameof(Enums.AssetTypes.Bond), Description = "Облигации" },
        new() {Id = (int) Enums.AssetTypes.Fund, Name = nameof(Enums.AssetTypes.Fund), Description = "Фонды" },
        new() {Id = (int) Enums.AssetTypes.CurrencyFiat, Name = nameof(Enums.AssetTypes.CurrencyFiat), Description = "Фиатные валюты" },
        new() {Id = (int) Enums.AssetTypes.CurrencyToken, Name = nameof(Enums.AssetTypes.CurrencyToken), Description = "Электронные валюты" },
        new() {Id = (int) Enums.AssetTypes.NftToken, Name = nameof(Enums.AssetTypes.NftToken), Description = "NFT"},
        new() {Id = (int) Enums.AssetTypes.RealEstate, Name = nameof(Enums.AssetTypes.RealEstate), Description = "Недвижимое имущество"},
        new() {Id = (int) Enums.AssetTypes.PersonalEstate, Name = nameof(Enums.AssetTypes.PersonalEstate), Description = "Движимое имущество"}
    };

    public static readonly Catalog[] Exchanges =
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

    public static readonly Catalog[] Currencies =
    {
        new() {Id = (int) Enums.Currencies.Default, Name = nameof(Enums.Currencies.Default), Description = "Не определено" },
        new() {Id = (int) Enums.Currencies.Rub, Name = nameof(Enums.Currencies.Rub), Description = "₽"},
        new() {Id = (int) Enums.Currencies.Usd, Name = nameof(Enums.Currencies.Usd), Description = "$"},
        new() {Id = (int) Enums.Currencies.Eur, Name = nameof(Enums.Currencies.Eur), Description = "€"},
        new() {Id = (int) Enums.Currencies.Gbp, Name = nameof(Enums.Currencies.Gbp), Description = "£"},
        new() {Id = (int) Enums.Currencies.Chy, Name = nameof(Enums.Currencies.Chy), Description = "¥"}
    };

    public static readonly Catalog[] Countries =
    {
        new() { Id = (int) Enums.Countries.Default, Name = nameof(Enums.Countries.Default), Description = "Не определено" },
        new() { Id = (int) Enums.Countries.Rus, Name = nameof(Enums.Countries.Rus), Description = "Russia" },
        new() { Id = (int) Enums.Countries.Usa, Name = nameof(Enums.Countries.Usa), Description = "USA" },
        new() { Id = (int) Enums.Countries.Chn, Name = nameof(Enums.Countries.Chn), Description = "China" },
        new() { Id = (int) Enums.Countries.Deu, Name = nameof(Enums.Countries.Deu), Description = "Deutschland" },
        new() { Id = (int) Enums.Countries.Gbr, Name = nameof(Enums.Countries.Gbr), Description = "Great Britain" },
        new() { Id = (int) Enums.Countries.Che, Name = nameof(Enums.Countries.Che), Description = "Switzerland" },
        new() { Id = (int) Enums.Countries.Jpn, Name = nameof(Enums.Countries.Jpn), Description = "Japan" }
    };
}