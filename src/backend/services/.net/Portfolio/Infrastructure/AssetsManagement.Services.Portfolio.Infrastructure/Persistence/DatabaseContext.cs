using AM.Services.Common.Contracts.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Operations;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

using Microsoft.EntityFrameworkCore;

using Shared.Persistense.Abstractions.Context;
using Shared.Persistense.Entities;
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
    #endregion
    #region Operations
    public DbSet<Income> Incomes { get; set; } = null!;
    public DbSet<Expense> Expenses { get; set; } = null!;
    #endregion
    #region States
    public DbSet<Asset> Assets { get; set; } = null!;
    public DbSet<Derivative> Derivatives { get; set; } = null!;
    public DbSet<Deal> Deals { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<Report> Reports { get; set; } = null!;
    #endregion
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<StringId> StringIds { get; set; } = null!;

    public DatabaseContext(DbContextOptions<DbContext> options) : base(options)
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

        builder.Entity<AssetType>().HasData(Common.Contracts.Constants.Persistense.Catalogs.AssetTypes);
        builder.Entity<Country>().HasData(Common.Contracts.Constants.Persistense.Catalogs.Countries);
        builder.Entity<Exchange>().HasData(Common.Contracts.Constants.Persistense.Catalogs.Exchanges);

        builder.Entity<EventType>().HasData(
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Default,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Default,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Default),
                Description = "Не определено"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Increase,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Increase),
                Description = "Увеличение баланса актива"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Decrease,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Decrease),
                Description = "Уменьшение баланса актива"
            },

            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.BankInvestments,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.BankInvestments),
                Description = "Ивестиции в банковские продукты"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.CrowdfundingInvestments,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.CrowdfundingInvestments),
                Description = "Инвестиции в краудфандинг"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.CrowdlendingInvestments,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.CrowdlendingInvestments),
                Description = "Инвестиции в краудлендинг"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.VentureInvestments,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.VentureInvestments),
                Description = "Венчурные инвестиции"
            },

            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.InterestIncome,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.InterestIncome),
                Description = "Процентная прибыль"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.InvestmentIncome,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.InvestmentIncome),
                Description = "Инвестиционная прибыль"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.InvestmentBody,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.InvestmentBody),
                Description = "Возврат тела инвестиции"
            },

            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Split,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Split),
                Description = "Увеличение актива за счет его диления"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Coupon,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Coupon),
                Description = "Купоны по активу"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Dividend,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Dividend),
                Description = "Дивиденды по активу"
            },

            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Delisting,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Delisting),
                Description = "Исключение актива из списка"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Loss,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Loss),
                Description = "Регистрация убытка"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.TaxIncome,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.TaxIncome),
                Description = "Налог на доход по активу"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.Ndfl,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.Ndfl),
                Description = "НДФЛ"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.TaxDeal,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.TaxDeal),
                Description = "Комиссия за проведение сделки"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.TaxProvider,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.TaxProvider),
                Description = "Комиссия поставщику актива"
            },
            new()
            {
                Id = (int)Core.Constants.Persistense.Enums.EventTypes.TaxDepositary,
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Name = nameof(Core.Constants.Persistense.Enums.EventTypes.TaxDepositary),
                Description = "Комиссия депозитарию актива"
            });
        builder.Entity<OperationType>().HasData(
            new() { Id = (int)Core.Constants.Persistense.Enums.OperationTypes.Default, Name = nameof(Core.Constants.Persistense.Enums.OperationTypes.Default), Description = "Не определено" },
            new() { Id = (int)Core.Constants.Persistense.Enums.OperationTypes.Income, Name = nameof(Core.Constants.Persistense.Enums.OperationTypes.Income), Description = "Приход" },
            new() { Id = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense, Name = nameof(Core.Constants.Persistense.Enums.OperationTypes.Expense), Description = "Расход" });
        builder.Entity<Provider>().HasData(
            new() { Id = (int)Core.Constants.Persistense.Enums.Providers.Safe, Name = nameof(Core.Constants.Persistense.Enums.Providers.Safe), Description = "Приватное хранение" },
            new() { Id = (int)Core.Constants.Persistense.Enums.Providers.Bcs, Name = "БКС", Description = "Брокер-банк" },
            new() { Id = (int)Core.Constants.Persistense.Enums.Providers.Tinkoff, Name = "Тинкофф", Description = "Банк-брокер" },
            new() { Id = (int)Core.Constants.Persistense.Enums.Providers.Vtb, Name = "ВТБ", Description = "Банк-брокер" },
            new() { Id = (int)Core.Constants.Persistense.Enums.Providers.Bitokk, Name = nameof(Core.Constants.Persistense.Enums.Providers.Bitokk), Description = "Криптообменник https://bitokk.biz/" },
            new() { Id = (int)Core.Constants.Persistense.Enums.Providers.XChange, Name = nameof(Core.Constants.Persistense.Enums.Providers.XChange), Description = "Криптообменник https://xchange.cash/" },
            new() { Id = (int)Core.Constants.Persistense.Enums.Providers.JetLend, Name = nameof(Core.Constants.Persistense.Enums.Providers.JetLend), Description = "Краудлендинговая платформа https://jetlend.ru/" });
        #endregion

        builder.Entity<Account>().HasIndex(x => new { x.Name, x.UserId, x.ProviderId }).IsUnique();
        builder.Entity<Asset>().HasKey(x => new { x.Id, x.AssetTypeId });
        builder.Entity<Derivative>().HasKey(x => new { x.Id, x.Code });
        builder.Entity<Report>().HasIndex(x => new { x.Name, x.UserId, x.ProviderId }).IsUnique();
    }
}