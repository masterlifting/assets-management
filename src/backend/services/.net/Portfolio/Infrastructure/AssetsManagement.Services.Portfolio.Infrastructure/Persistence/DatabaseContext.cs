using AM.Services.Common.Contracts.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Operations;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Persistense.Abstractions.Entities.State.Handle;
using Shared.Infrastructure.Persistense.Entities;

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
        builder.Entity<AssetType>().HasData(Catalogs.AssetTypes);
        builder.Entity<Country>().HasData(Catalogs.Countries);
        builder.Entity<Exchange>().HasData(Catalogs.Exchanges);
        builder.Entity<State>().HasData(Shared.Infrastructure.Persistense.Constants.States);
        
        builder.Entity<EventType>().HasData(
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.Default,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Default,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.Default),
                Description = "Не определено"
            },
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.Increase,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Income,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.Increase),
                Description = "Увеличение баланса актива"
            },
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.Decrease,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Expense,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.Decrease),
                Description = "Уменьшение баланса актива"
            },

            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.BankInvestments,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Expense,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.BankInvestments),
                Description = "Ивестиции в банковские продукты"
            },
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.CrowdfundingInvestments,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Expense,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.CrowdfundingInvestments),
                Description = "Инвестиции в краудфандинг"
            },
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.CrowdlendingInvestments,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Expense,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.CrowdlendingInvestments),
                Description = "Инвестиции в краудлендинг"
            },
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.VentureInvestments,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Expense,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.VentureInvestments),
                Description = "Венчурные инвестиции"
            },
            
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.InterestIncome,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Income,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.InterestIncome),
                Description = "Процентная прибыль"
            },
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.InvestmentIncome,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Income,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.InvestmentIncome),
                Description = "Инвестиционная прибыль"
            },
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.InvestmentBody,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Income,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.InvestmentBody),
                Description = "Возврат тела инвестиции"
            },
            
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.Split,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Income,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.Split),
                Description = "Увеличение актива за счет его диления"
            },
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.Coupon,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Income,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.Coupon),
                Description = "Купоны по активу"
            },
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.Dividend,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Income,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.Dividend),
                Description = "Дивиденды по активу"
            },

            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.Delisting,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Expense,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.Delisting),
                Description = "Исключение актива из списка"
            },
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.Loss,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Expense,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.Loss),
                Description = "Регистрация убытка"
            },
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.TaxIncome,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Expense,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.TaxIncome),
                Description = "Налог на доход по активу"
            },
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.Ndfl,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Expense,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.Ndfl),
                Description = "НДФЛ"
            },
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.TaxDeal,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Expense,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.TaxDeal),
                Description = "Комиссия за проведение сделки"
            },
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.TaxProvider,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Expense,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.TaxProvider),
                Description = "Комиссия поставщику актива"
            },
            new()
            {
                Id = (int)Core.Domain.Persistense.Entities.Enums.EventTypes.TaxDepositary,
                OperationTypeId = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Expense,
                Name = nameof(Core.Domain.Persistense.Entities.Enums.EventTypes.TaxDepositary),
                Description = "Комиссия депозитарию актива"
            });
        builder.Entity<OperationType>().HasData(
            new() { Id = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Default, Name = nameof(Core.Domain.Persistense.Entities.Enums.OperationTypes.Default), Description = "Не определено" },
            new() { Id = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Income, Name = nameof(Core.Domain.Persistense.Entities.Enums.OperationTypes.Income), Description = "Приход" },
            new() { Id = (int)Core.Domain.Persistense.Entities.Enums.OperationTypes.Expense, Name = nameof(Core.Domain.Persistense.Entities.Enums.OperationTypes.Expense), Description = "Расход" });
        builder.Entity<Provider>().HasData(
            new() { Id = (int)Core.Domain.Persistense.Entities.Enums.Providers.Safe, Name = nameof(Core.Domain.Persistense.Entities.Enums.Providers.Safe), Description = "Приватное хранение" },
            new() { Id = (int)Core.Domain.Persistense.Entities.Enums.Providers.Bcs, Name = "БКС", Description = "Брокер-банк" },
            new() { Id = (int)Core.Domain.Persistense.Entities.Enums.Providers.Tinkoff, Name = "Тинкофф", Description = "Банк-брокер" },
            new() { Id = (int)Core.Domain.Persistense.Entities.Enums.Providers.Vtb, Name = "ВТБ", Description = "Банк-брокер" },
            new() { Id = (int)Core.Domain.Persistense.Entities.Enums.Providers.Bitokk, Name =nameof(Core.Domain.Persistense.Entities.Enums.Providers.Bitokk), Description = "Криптообменник https://bitokk.biz/" },
            new() { Id = (int)Core.Domain.Persistense.Entities.Enums.Providers.XChange, Name =nameof(Core.Domain.Persistense.Entities.Enums.Providers.XChange), Description = "Криптообменник https://xchange.cash/" },
            new() { Id = (int)Core.Domain.Persistense.Entities.Enums.Providers.JetLend, Name = nameof(Core.Domain.Persistense.Entities.Enums.Providers.JetLend), Description = "Краудлендинговая платформа https://jetlend.ru/" });
        builder.Entity<Step>().HasData(
            new() {Id = (int) Core.Domain.Persistense.Entities.Enums.Steps.Parsing, Name = nameof(Core.Domain.Persistense.Entities.Enums.Steps.Parsing), Description = "Парсинг" },
            new() {Id = (int) Core.Domain.Persistense.Entities.Enums.Steps.Calculating, Name = nameof(Core.Domain.Persistense.Entities.Enums.Steps.Calculating), Description = "Расчет" }, 
            new() {Id = (int) Core.Domain.Persistense.Entities.Enums.Steps.Sending, Name = nameof(Core.Domain.Persistense.Entities.Enums.Steps.Sending), Description = "Отправка" });
        #endregion
        
        builder.Entity<Account>().HasIndex(x => new { x.Name, x.UserId, x.ProviderId }).IsUnique();
        builder.Entity<Asset>().HasKey(x => new { x.Id, x.AssetTypeId });
        builder.Entity<Derivative>().HasKey(x => new { x.Id, x.Code });
        builder.Entity<Report>().HasIndex(x => new {x.Name, x.UserId, x.ProviderId}).IsUnique();
    }
}
