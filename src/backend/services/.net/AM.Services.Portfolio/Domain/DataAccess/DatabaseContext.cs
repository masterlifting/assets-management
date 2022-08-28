using AM.Services.Common.Contracts.Models.Entity;
using AM.Services.Portfolio.Domain.Entities;
using AM.Services.Portfolio.Domain.Entities.Catalogs;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Domains.Entities;

namespace AM.Services.Portfolio.Domain.DataAccess;

public sealed class DatabaseContext : DbContext
{
    public DbSet<AssetType> AssetTypes { get; set; } = null!;
    public DbSet<Asset> Assets { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;

    public DbSet<Derivative> Derivatives { get; set; } = null!;

    public DbSet<Income> Incomes { get; set; } = null!;
    public DbSet<Expense> Expenses { get; set; } = null!;
    public DbSet<Deal> Deals { get; set; } = null!;
    public DbSet<OperationType> OperationTypes { get; set; } = null!;
    public DbSet<EventType> EventTypes { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;

    public DbSet<Report> Reports { get; set; } = null!;
    public DbSet<ReportFile> ReportFiles { get; set; } = null!;

    public DbSet<Provider> Providers { get; set; } = null!;
    public DbSet<Exchange> Exchanges { get; set; } = null!;
    public DbSet<Country> Countries { get; set; } = null!;

    public DbSet<LongId> LongIds { get; set; } = null!;

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        //Database.EnsureDeleted();
        //Database.EnsureCreated();
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.UseSerialColumns();

        builder.Entity<Asset>().HasKey(x => new { x.Id, x.TypeId });
        builder.Entity<AssetType>().HasData(Catalogs.AssetTypes);
        builder.Entity<Country>().HasData(Catalogs.Countries);

        builder.Entity<Exchange>().HasData(Catalogs.Exchanges);

        builder.Entity<Derivative>().HasKey(x => new { x.Id, x.Code });
        builder.Entity<Account>().HasIndex(x => new { x.Name, x.UserId, x.ProviderId }).IsUnique();
        builder.Entity<ReportFile>().HasIndex(x => new {x.Name, x.UserId, x.ProviderId}).IsUnique();

        builder.Entity<Provider>().HasData(
            new() { Id = (int)Enums.Providers.Safe, Name = nameof(Enums.Providers.Safe), Description = "Приватное хранение" },
            new() { Id = (int)Enums.Providers.BCS, Name = "БКС", Description = "Брокер-банк" },
            new() { Id = (int)Enums.Providers.Tinkoff, Name = "Тинкофф", Description = "Банк-брокер" },
            new() { Id = (int)Enums.Providers.VTB, Name = "ВТБ", Description = "Банк-брокер" },
            new() { Id = (int)Enums.Providers.Bitokk, Name =nameof(Enums.Providers.Bitokk), Description = "Криптообменник https://bitokk.biz/" },
            new() { Id = (int)Enums.Providers.XChange, Name =nameof(Enums.Providers.XChange), Description = "Криптообменник https://xchange.cash/" },
            new() { Id = (int)Enums.Providers.JetLend, Name = nameof(Enums.Providers.JetLend), Description = "Краудлендинговая платформа https://jetlend.ru/" });

        builder.Entity<OperationType>().HasData(
            new() { Id = (byte)Enums.OperationTypes.Default, Name = nameof(Enums.OperationTypes.Default), Description = "Не определено" },
            new() { Id = (byte)Enums.OperationTypes.Income, Name = nameof(Enums.OperationTypes.Income), Description = "Приход" },
            new() { Id = (byte)Enums.OperationTypes.Expense, Name = nameof(Enums.OperationTypes.Expense), Description = "Расход" });

        builder.Entity<EventType>().HasData(
            new()
            {
                Id = (byte)Enums.EventTypes.Default,
                OperationTypeId = (byte)Enums.OperationTypes.Default,
                Name = nameof(Enums.EventTypes.Default),
                Description = "Не определено"
            },
            new()
            {
                Id = (byte)Enums.EventTypes.Increase,
                OperationTypeId = (byte)Enums.OperationTypes.Income,
                Name = nameof(Enums.EventTypes.Increase),
                Description = "Увеличение баланса актива"
            },
            new()
            {
                Id = (byte)Enums.EventTypes.Decrease,
                OperationTypeId = (byte)Enums.OperationTypes.Expense,
                Name = nameof(Enums.EventTypes.Decrease),
                Description = "Уменьшение баланса актива"
            },

            new()
            {
                Id = (byte)Enums.EventTypes.BankInvestments,
                OperationTypeId = (byte)Enums.OperationTypes.Expense,
                Name = nameof(Enums.EventTypes.BankInvestments),
                Description = "Ивестиции в банковские продукты"
            },
            new()
            {
                Id = (byte)Enums.EventTypes.CrowdfundingInvestments,
                OperationTypeId = (byte)Enums.OperationTypes.Expense,
                Name = nameof(Enums.EventTypes.CrowdfundingInvestments),
                Description = "Инвестиции в краудфандинг"
            },
            new()
            {
                Id = (byte)Enums.EventTypes.CrowdlendingInvestments,
                OperationTypeId = (byte)Enums.OperationTypes.Expense,
                Name = nameof(Enums.EventTypes.CrowdlendingInvestments),
                Description = "Инвестиции в краудлендинг"
            },
            new()
            {
                Id = (byte)Enums.EventTypes.VentureInvestments,
                OperationTypeId = (byte)Enums.OperationTypes.Expense,
                Name = nameof(Enums.EventTypes.VentureInvestments),
                Description = "Венчурные инвестиции"
            },
            
            new()
            {
                Id = (byte)Enums.EventTypes.InterestIncome,
                OperationTypeId = (byte)Enums.OperationTypes.Income,
                Name = nameof(Enums.EventTypes.InterestIncome),
                Description = "Процентная прибыль"
            },
            new()
            {
                Id = (byte)Enums.EventTypes.InvestmentIncome,
                OperationTypeId = (byte)Enums.OperationTypes.Income,
                Name = nameof(Enums.EventTypes.InvestmentIncome),
                Description = "Инвестиционная прибыль"
            },
            new()
            {
                Id = (byte)Enums.EventTypes.InvestmentBody,
                OperationTypeId = (byte)Enums.OperationTypes.Income,
                Name = nameof(Enums.EventTypes.InvestmentBody),
                Description = "Возврат тела инвестиции"
            },
            
            new()
            {
                Id = (byte)Enums.EventTypes.Split,
                OperationTypeId = (byte)Enums.OperationTypes.Income,
                Name = nameof(Enums.EventTypes.Split),
                Description = "Увеличение актива за счет его диления"
            },
            new()
            {
                Id = (byte)Enums.EventTypes.Coupon,
                OperationTypeId = (byte)Enums.OperationTypes.Income,
                Name = nameof(Enums.EventTypes.Coupon),
                Description = "Купоны по активу"
            },
            new()
            {
                Id = (byte)Enums.EventTypes.Dividend,
                OperationTypeId = (byte)Enums.OperationTypes.Income,
                Name = nameof(Enums.EventTypes.Dividend),
                Description = "Дивиденды по активу"
            },

            new()
            {
                Id = (byte)Enums.EventTypes.Delisting,
                OperationTypeId = (byte)Enums.OperationTypes.Expense,
                Name = nameof(Enums.EventTypes.Delisting),
                Description = "Исключение актива из списка"
            },
            new()
            {
                Id = (byte)Enums.EventTypes.Loss,
                OperationTypeId = (byte)Enums.OperationTypes.Expense,
                Name = nameof(Enums.EventTypes.Loss),
                Description = "Регистрация убытка"
            },
            new()
            {
                Id = (byte)Enums.EventTypes.TaxIncome,
                OperationTypeId = (byte)Enums.OperationTypes.Expense,
                Name = nameof(Enums.EventTypes.TaxIncome),
                Description = "Налог на доход по активу"
            },
            new()
            {
                Id = (byte)Enums.EventTypes.NDFL,
                OperationTypeId = (byte)Enums.OperationTypes.Expense,
                Name = nameof(Enums.EventTypes.NDFL),
                Description = "НДФЛ"
            },
            new()
            {
                Id = (byte)Enums.EventTypes.TaxDeal,
                OperationTypeId = (byte)Enums.OperationTypes.Expense,
                Name = nameof(Enums.EventTypes.TaxDeal),
                Description = "Комиссия за проведение сделки"
            },
            new()
            {
                Id = (byte)Enums.EventTypes.TaxProvider,
                OperationTypeId = (byte)Enums.OperationTypes.Expense,
                Name = nameof(Enums.EventTypes.TaxProvider),
                Description = "Комиссия поставщику актива"
            },
            new()
            {
                Id = (byte)Enums.EventTypes.TaxDepositary,
                OperationTypeId = (byte)Enums.OperationTypes.Expense,
                Name = nameof(Enums.EventTypes.TaxDepositary),
                Description = "Комиссия депозитарию актива"
            });

        base.OnModelCreating(builder);
    }
}
