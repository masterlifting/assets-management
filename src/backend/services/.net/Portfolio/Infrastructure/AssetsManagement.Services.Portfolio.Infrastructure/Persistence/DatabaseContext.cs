using AM.Services.Common.Contracts.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

using Microsoft.EntityFrameworkCore;

using Shared.Persistense.Abstractions.Context;
using Shared.Persistense.Entities;
using Shared.Persistense.Entities.EntityFile;
using Shared.Persistense.Entities.EntityState;

namespace AM.Services.Portfolio.Infrastructure.Persistence;

public sealed class DatabaseContext : DbContext, IEntityStateDbContext
{
    #region Catalogs
    public DbSet<AssetType> AssetTypes { get; set; } = null!;
    public DbSet<EventType> EventTypes { get; set; } = null!;
    public DbSet<OperationType> OperationTypes { get; set; } = null!;
    public DbSet<Country> Countries { get; set; } = null!;
    public DbSet<Exchange> Exchanges { get; set; } = null!;
    public DbSet<Provider> Providers { get; set; } = null!;

    public DbSet<State> States { get; set; } = null!;
    public DbSet<Step> Steps { get; set; } = null!;
    public DbSet<ContentType> ContentTypes { get; set; } = null!;
    #endregion
    #region States
    public DbSet<Asset> Assets { get; set; } = null!;
    public DbSet<Derivative> Derivatives { get; set; } = null!;
    public DbSet<Deal> Deals { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<ReportFile> Reports { get; set; } = null!;
    #endregion
    public DbSet<Income> Incomes { get; set; } = null!;
    public DbSet<Expense> Expenses { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<StringId> StringIds { get; set; } = null!;

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        //Database.EnsureDeleted();
        //Database.EnsureCreated();
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.UseSerialColumns();

        #region Catalogs
        builder.Entity<State>().HasData(Shared.Persistense.Constants.Catalogs.States);
        builder.Entity<Step>().HasData(Shared.Persistense.Constants.Catalogs.Steps);
        builder.Entity<ContentType>().HasData(Shared.Persistense.Constants.Catalogs.ContentTypes);

        builder.Entity<AssetType>().HasData(Common.Contracts.Constants.Persistense.Catalogs.AssetTypes);
        builder.Entity<Country>().HasData(Common.Contracts.Constants.Persistense.Catalogs.Countries);
        builder.Entity<Exchange>().HasData(Common.Contracts.Constants.Persistense.Catalogs.Exchanges);

        builder.Entity<EventType>().HasData(
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Default,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Default,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Default),
                Info = "Не определено"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Increase,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Increase),
                Info = "Увеличение баланса актива"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Decrease,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Decrease),
                Info = "Уменьшение баланса актива"
            },

            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.BankInvestments,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.BankInvestments),
                Info = "Инвестиции в банковские продукты"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.CrowdfundingInvestments,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.CrowdfundingInvestments),
                Info = "Инвестиции в краудфандинг"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.CrowdlendingInvestments,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.CrowdlendingInvestments),
                Info = "Инвестиции в краудлендинг"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.VentureInvestments,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.VentureInvestments),
                Info = "Венчурные инвестиции"
            },

            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.InterestIncome,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.InterestIncome),
                Info = "Процентная прибыль"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.InvestmentIncome,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.InvestmentIncome),
                Info = "Инвестиционная прибыль"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.InvestmentBody,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.InvestmentBody),
                Info = "Возврат тела инвестиции"
            },

            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Split,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Split),
                Info = "Увеличение актива за счет его диления"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Coupon,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Coupon),
                Info = "Купоны по активу"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Dividend,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Dividend),
                Info = "Дивиденды по активу"
            },

            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Delisting,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Delisting),
                Info = "Исключение актива из списка"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Loss,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Loss),
                Info = "Регистрация убытка"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.TaxIncome,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.TaxIncome),
                Info = "Налог на доход по активу"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Ndfl,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Ndfl),
                Info = "НДФЛ"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.TaxDeal,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.TaxDeal),
                Info = "Комиссия за проведение сделки"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.TaxProvider,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.TaxProvider),
                Info = "Комиссия поставщику актива"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.TaxDepositary,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.TaxDepositary),
                Info = "Комиссия депозитарию актива"
            });
        builder.Entity<OperationType>().HasData(
            new() { Id = (int)Core.Constants.Persistense.Enums.OperationTypes.Default, Name = nameof(Core.Constants.Persistense.Enums.OperationTypes.Default), Info = "Не определено" },
            new() { Id = (int)Core.Constants.Persistense.Enums.OperationTypes.Income, Name = nameof(Core.Constants.Persistense.Enums.OperationTypes.Income), Info = "Приход" },
            new() { Id = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense, Name = nameof(Core.Constants.Persistense.Enums.OperationTypes.Expense), Info = "Расход" });
        builder.Entity<Provider>().HasData(
            new() { Id = (int)Core.Constants.Persistense.Enums.Providers.Safe, Name = nameof(Core.Constants.Persistense.Enums.Providers.Safe), Info = "Приватное хранение" },
            new() { Id = (int)Core.Constants.Persistense.Enums.Providers.Bcs, Name = "БКС", Info = "Брокер-банк" },
            new() { Id = (int)Core.Constants.Persistense.Enums.Providers.Tinkoff, Name = "Тинкофф", Info = "Банк-брокер" },
            new() { Id = (int)Core.Constants.Persistense.Enums.Providers.Vtb, Name = "ВТБ", Info = "Банк-брокер" },
            new() { Id = (int)Core.Constants.Persistense.Enums.Providers.Bitokk, Name = nameof(Core.Constants.Persistense.Enums.Providers.Bitokk), Info = "Криптообменник https://bitokk.biz/" },
            new() { Id = (int)Core.Constants.Persistense.Enums.Providers.XChange, Name = nameof(Core.Constants.Persistense.Enums.Providers.XChange), Info = "Криптообменник https://xchange.cash/" },
            new() { Id = (int)Core.Constants.Persistense.Enums.Providers.JetLend, Name = nameof(Core.Constants.Persistense.Enums.Providers.JetLend), Info = "Краудлендинговая платформа https://jetlend.ru/" });
        #endregion

        builder.Entity<Asset>().HasKey(x => new { x.Id, x.AssetTypeId });
        builder.Entity<Derivative>().HasKey(x => new { x.Id, x.Code });

        builder.Entity<Account>().HasIndex(x => new { x.Name, x.UserId, x.ProviderId }).IsUnique();
        builder.Entity<ReportFile>().HasIndex(x => new { x.Name, x.UserId, x.ProviderId }).IsUnique();
    }
}