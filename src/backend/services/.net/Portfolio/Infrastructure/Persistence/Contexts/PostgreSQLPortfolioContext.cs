﻿using AM.Services.Common.Contracts.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;
using AM.Services.Portfolio.Infrastructure.Settings;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Shared.Persistence;
using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Abstractions.Entities.Catalogs;
using Shared.Persistence.Contexts;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Contexts;

public sealed class PostgreSQLPortfolioContext : PostgreSQLContext
{
    #region Catalogs
    public DbSet<AssetType> AssetTypes { get; set; } = null!;
    public DbSet<EventType> EventTypes { get; set; } = null!;
    public DbSet<OperationType> OperationTypes { get; set; } = null!;
    public DbSet<Country> Countries { get; set; } = null!;
    public DbSet<Exchange> Exchanges { get; set; } = null!;
    public DbSet<Provider> Providers { get; set; } = null!;
    public DbSet<ProcessStatus> States { get; set; } = null!;
    public DbSet<ProcessStep> Steps { get; set; } = null!;
    #endregion

    public DbSet<Asset> Assets { get; set; } = null!;
    public DbSet<Derivative> Derivatives { get; set; } = null!;
    public DbSet<Deal> Deals { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<Data> ReportData { get; set; } = null!;
    public DbSet<Income> Incomes { get; set; } = null!;
    public DbSet<Expense> Expenses { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    public PostgreSQLPortfolioContext(IOptions<DatabaseConnectionSection> options) : base(options.Value.PostgreSQL)
    {
        //Database.EnsureDeleted();
        //Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.UseSerialColumns();

        builder.Entity<GuidId>().HasNoKey();

        builder.Entity<Asset>().HasKey(x => new { x.Id, x.TypeId });
        builder.Entity<Derivative>().HasKey(x => new { x.Id, x.Code });

        builder.Entity<Account>().HasIndex(x => new { x.Name, x.UserId, x.ProviderId }).IsUnique();

        builder.Entity<PersistentCatalog>().HasKey(x => x.Id);


        #region Catalogs
        builder.Entity<Status>().HasData(Constants.Catalogs.Statuses);
        builder.Entity<Step>().HasData(Constants.Catalogs.Steps);
        builder.Entity<ContentType>().HasData(Constants.Catalogs.ContentTypes);

        builder.Entity<AssetType>().HasData(Common.Contracts.Constants.Persistense.Catalogs.AssetTypes);
        builder.Entity<Country>().HasData(Common.Contracts.Constants.Persistense.Catalogs.Countries);
        builder.Entity<Exchange>().HasData(Common.Contracts.Constants.Persistense.Catalogs.Exchanges);

        builder.Entity<EventType>().HasData(
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.Adding))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Info = "Increasing the asset balance"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.Withdrawing))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Decreasing the asset balance"
            },

            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.BankInvestments))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Investing in bank products"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.CrowdfundingInvestments))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Investing in crowdfunding"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.CrowdlendingInvestments))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Investing in crowdlending"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.VentureInvestments))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Venture investments"
            },

            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.InterestProfit))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Info = "Interest profit"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.InvestmentProfit))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Info = "Investment profit"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.InvestmentBody))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Info = "Returning of investment body"
            },

            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.Splitting))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Info = "Increasing an asset by dividing it"
            },

            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.Donation))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Info = "Increasing an asset by donation it"
            },

            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.Coupon))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Info = "Coupons from assets"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.Dividend))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Info = "Dividends from assets"
            },

            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.Delisting))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Excluding an asset from lists"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.Loss))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Loss registration"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.TaxIncome))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Tax on profit by an asset"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.TaxCountry))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Internal tax of country"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.TaxDeal))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Tax for a deal"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.TaxProvider))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Tax to provider"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.TaxDepositary))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Tax to depositary of an asset"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.ComissionDeal))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Commission for a deal"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.ComissionProvider))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Commission to provider"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.ComissionDepositary))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Commission to depositary of an asset"
            });
        builder.Entity<OperationType>().HasData(
            new(new OperationTypeId(Core.Constants.Persistense.Enums.OperationTypes.Income)) { Info = "Income" },
            new(new OperationTypeId(Core.Constants.Persistense.Enums.OperationTypes.Expense)) { Info = "Expense" });
        builder.Entity<Provider>().HasData(
            new(new ProviderId(Core.Constants.Persistense.Enums.Providers.Safe)) { Info = "Private storage" },
            new(new ProviderId(Core.Constants.Persistense.Enums.Providers.Bcs)) { Info = "Broker/Bank" },
            new(new ProviderId(Core.Constants.Persistense.Enums.Providers.Tinkoff)) { Info = "Broker/Bank" },
            new(new ProviderId(Core.Constants.Persistense.Enums.Providers.Vtb)) { Info = "Broker/Bank" },
            new(new ProviderId(Core.Constants.Persistense.Enums.Providers.Bitokk)) { Info = "Сrypto exchange https://bitokk.biz/" },
            new(new ProviderId(Core.Constants.Persistense.Enums.Providers.XChange)) { Info = "Сrypto exchange https://xchange.cash/" },
            new(new ProviderId(Core.Constants.Persistense.Enums.Providers.JetLend)) { Info = "Crowdlending platform https://jetlend.ru/" });
        #endregion
    }
}