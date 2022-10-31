using AM.Services.Common.Contracts.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

using Microsoft.EntityFrameworkCore;

using Shared.Persistense.Abstractions.Context;
using Shared.Persistense.Entities;
using Shared.Persistense.Entities.EntityData;
using Shared.Persistense.Entities.EntityState;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Context;

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
    public DbSet<ReportData> ReportData { get; set; } = null!;
    #endregion
    public DbSet<Report> Reports { get; set; } = null!;
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

        builder.Entity<StringId>().HasNoKey();

        builder.Entity<Asset>().HasKey(x => new { x.Id, x.TypeId });
        builder.Entity<Derivative>().HasKey(x => new { x.Id, x.Code });

        builder.Entity<Account>().HasIndex(x => new { x.Name, x.UserId, x.ProviderId }).IsUnique();

        #region Catalogs
        builder.Entity<State>().HasData(Shared.Persistense.Constants.Catalogs.States);
        builder.Entity<Step>().HasData(Shared.Persistense.Constants.Catalogs.Steps);
        builder.Entity<ContentType>().HasData(Shared.Persistense.Constants.Catalogs.ContentTypes);

        builder.Entity<AssetType>().HasData(Common.Contracts.Constants.Persistense.Catalogs.AssetTypes);
        builder.Entity<Country>().HasData(Common.Contracts.Constants.Persistense.Catalogs.Countries);
        builder.Entity<Exchange>().HasData(Common.Contracts.Constants.Persistense.Catalogs.Exchanges);

        builder.Entity<EventType>().HasData(
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.Default))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Default,
                Info = "Not defined"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.Increase))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Info = "Increase in asset balance"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.Decrease))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Decrease in asset balance"
            },

            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.BankInvestments))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Investments in bank products"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.CrowdfundingInvestments))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Investments in crowdfunding"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.CrowdlendingInvestments))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Investments in crowdlending"
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

            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.Split))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Income,
                Info = "Increasing an asset by dividing it"
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
                Info = "Commission for a deal"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.TaxProvider))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Commission to provider"
            },
            new(new EventTypeId(Core.Constants.Persistense.Enums.EventTypes.TaxDepositary))
            {
                OperationTypeId = (int)Core.Constants.Persistense.Enums.OperationTypes.Expense,
                Info = "Commission to depositary of an asset"
            });
        builder.Entity<OperationType>().HasData(
            new(new OperationTypeId(Core.Constants.Persistense.Enums.OperationTypes.Default)) { Info = "Not defined" },
            new(new OperationTypeId(Core.Constants.Persistense.Enums.OperationTypes.Income)) { Info = "Income" },
            new(new OperationTypeId(Core.Constants.Persistense.Enums.OperationTypes.Expense)) { Info = "Expense" });
        builder.Entity<Provider>().HasData(
            new(new ProviderId(Core.Constants.Persistense.Enums.Providers.Safe)) { Info = "Private storage" },
            new(new ProviderId(Core.Constants.Persistense.Enums.Providers.Bcs)) { Info = "Broker/Bank" },
            new(new ProviderId(Core.Constants.Persistense.Enums.Providers.Tinkoff)) { Info = "Broker/Bank" },
            new(new ProviderId(Core.Constants.Persistense.Enums.Providers.Vtb)) { Info = "Broker/Bank" },
            new(new ProviderId(Core.Constants.Persistense.Enums.Providers.Bitokk)) { Info = "Сrypto exchange https://bitokk.biz/" },
            new(new ProviderId(Core.Constants.Persistense.Enums.Providers.XChange)){ Info = "Сrypto exchange https://xchange.cash/" },
            new(new ProviderId(Core.Constants.Persistense.Enums.Providers.JetLend)) { Info = "Crowdlending platform https://jetlend.ru/" });
        #endregion
    }
}