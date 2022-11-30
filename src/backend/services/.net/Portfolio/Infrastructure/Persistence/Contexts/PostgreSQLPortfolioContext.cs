using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;
using AM.Services.Portfolio.Infrastructure.Settings;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Shared.Persistence.Abstractions.Entities;
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
    public DbSet<ProcessStatus> ProcessStatuses { get; set; } = null!;
    public DbSet<ProcessStep> ProcessSteps { get; set; } = null!;
    #endregion

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<Asset> Assets { get; set; } = null!;
    public DbSet<Derivative> Derivatives { get; set; } = null!;
    public DbSet<Deal> Deals { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<Income> Incomes { get; set; } = null!;
    public DbSet<Expense> Expenses { get; set; } = null!;

    public PostgreSQLPortfolioContext(IOptions<DatabaseConnectionSection> options) : base(options.Value.PostgreSQL)
    {
        //Database.EnsureDeleted();
        //Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.UseSerialColumns();

        builder.Entity<GuidId>().HasNoKey();

        builder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Info).HasMaxLength(500);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
        });
        builder.Entity<Account>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Info).HasMaxLength(500);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);

            e.HasIndex(x => new { x.Name, x.UserId, x.ProviderId }).IsUnique();
        });
        builder.Entity<Event>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Value).HasPrecision(18, 10);
            e.Property(x => x.Info).HasMaxLength(500);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
            e.Property(x => x.Updated).HasDefaultValue(DateTime.UtcNow);
        });
        builder.Entity<Deal>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Cost).HasPrecision(18, 10);
            e.Property(x => x.Info).HasMaxLength(500);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
            e.Property(x => x.Updated).HasDefaultValue(DateTime.UtcNow);
        });
        builder.Entity<Asset>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.BalanceValue).HasPrecision(18, 10);
            e.Property(x => x.BalanceCost).HasPrecision(18, 10);
            e.Property(x => x.LastDealCost).HasPrecision(18, 10);
            e.Property(x => x.Info).HasMaxLength(500);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
            e.Property(x => x.Updated).HasDefaultValue(DateTime.UtcNow);

            e.HasIndex(x => new { x.Name, x.TypeId }).IsUnique();
        });
        builder.Entity<Derivative>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            e.Property(x => x.Code).IsRequired().HasMaxLength(50);
            e.Property(x => x.BalanceValue).HasPrecision(18, 10);
            e.Property(x => x.BalanceCost).HasPrecision(18, 10);
            e.Property(x => x.LastDealCost).HasPrecision(18, 10);
            e.Property(x => x.Info).HasMaxLength(500);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
            e.Property(x => x.Updated).HasDefaultValue(DateTime.UtcNow);

            e.HasIndex(x => new { x.Name, x.Code }).IsUnique();
        });
        builder.Entity<Income>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Value).HasPrecision(18, 10);
            e.Property(x => x.Info).HasMaxLength(500);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
        });
        builder.Entity<Expense>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Value).HasPrecision(18, 10);
            e.Property(x => x.Info).HasMaxLength(500);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
        });

        builder.Entity<ProcessStatus>(e => 
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.Name).IsUnique();
        });
        builder.Entity<ProcessStep>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.Name).IsUnique();
        });
        builder.Entity<AssetType>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.Name).IsUnique();
        });
        builder.Entity<Country>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.Name).IsUnique();
        });
        builder.Entity<Exchange>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.Name).IsUnique();
        });
        builder.Entity<Provider>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.Name).IsUnique();
        });
        builder.Entity<OperationType>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.Name).IsUnique();
        });
        builder.Entity<EventType>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.Name).IsUnique();
        });

        #region Catalogs
        //builder.Entity<Country>().HasData(new Country[]
        //{
        //    new() {Id = (int) AM.Services.Common.Contracts.Constants.Enums.Currencies.Rub, Name = nameof(AM.Services.Common.Contracts.Constants.Enums.Currencies.Rub), Info = "₽"},
        //    new() {Id = (int) AM.Services.Common.Contracts.Constants.Enums.Currencies.Usd, Name = nameof(AM.Services.Common.Contracts.Constants.Enums.Currencies.Usd), Info = "$"},
        //    new() {Id = (int) AM.Services.Common.Contracts.Constants.Enums.Currencies.Eur, Name = nameof(AM.Services.Common.Contracts.Constants.Enums.Currencies.Eur), Info = "€"},
        //    new() {Id = (int) AM.Services.Common.Contracts.Constants.Enums.Currencies.Gbp, Name = nameof(AM.Services.Common.Contracts.Constants.Enums.Currencies.Gbp), Info = "£"},
        //    new() {Id = (int) AM.Services.Common.Contracts.Constants.Enums.Currencies.Chy, Name = nameof(AM.Services.Common.Contracts.Constants.Enums.Currencies.Chy), Info = "¥"}
        //});
        builder.Entity<ProcessStatus>().HasData(new ProcessStatus[]
        {
            new(){Id = (int)Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Draft, Name = nameof(Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Draft), Info = "Draft" },
            new(){Id = (int)Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Ready, Name = nameof(Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Ready), Info = "Ready to processing data" },
            new(){Id = (int)Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Processing, Name = nameof(Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Processing), Info = "Processing data" },
            new(){Id = (int)Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Processed, Name = nameof(Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Processed), Info = "Processed data" },
            new(){Id = (int)Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Error, Name = nameof(Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Error), Info = "Error of processing" }
        });
        builder.Entity<ProcessStep>().HasData(new ProcessStep[]
        {
            new(){Id = (int)Core.Constants.Enums.ProcessSteps.ParseBcsReport, Name = nameof(Core.Constants.Enums.ProcessSteps.ParseBcsReport)}
        });
        builder.Entity<AssetType>().HasData(new AssetType[]
        {
            new() {Id = (int) Common.Contracts.Constants.Enums.AssetTypes.Valuable, Name = nameof(Common.Contracts.Constants.Enums.AssetTypes.Valuable), Info = "Valuable" },
            new() {Id = (int) Common.Contracts.Constants.Enums.AssetTypes.Stock, Name = nameof(Common.Contracts.Constants.Enums.AssetTypes.Stock), Info = "Stocks" },
            new() {Id = (int) Common.Contracts.Constants.Enums.AssetTypes.Bond, Name = nameof(Common.Contracts.Constants.Enums.AssetTypes.Bond), Info = "Bonds" },
            new() {Id = (int) Common.Contracts.Constants.Enums.AssetTypes.Fund, Name = nameof(Common.Contracts.Constants.Enums.AssetTypes.Fund), Info = "Founds" },
            new() {Id = (int) Common.Contracts.Constants.Enums.AssetTypes.CurrencyFiat, Name = nameof(Common.Contracts.Constants.Enums.AssetTypes.CurrencyFiat), Info = "Fiat currencies" },
            new() {Id = (int) Common.Contracts.Constants.Enums.AssetTypes.CurrencyToken, Name = nameof(Common.Contracts.Constants.Enums.AssetTypes.CurrencyToken), Info = "Crypto currencies" },
            new() {Id = (int) Common.Contracts.Constants.Enums.AssetTypes.NftToken, Name = nameof(Common.Contracts.Constants.Enums.AssetTypes.NftToken), Info = "NFT tokens"},
            new() {Id = (int) Common.Contracts.Constants.Enums.AssetTypes.RealEstate, Name = nameof(Common.Contracts.Constants.Enums.AssetTypes.RealEstate), Info = "Real estates"},
            new() {Id = (int) Common.Contracts.Constants.Enums.AssetTypes.PersonalEstate, Name = nameof(Common.Contracts.Constants.Enums.AssetTypes.PersonalEstate), Info = "Personal estates"}
        });
        builder.Entity<Country>().HasData(new Country[]
        {
            new() { Id = (int) Common.Contracts.Constants.Enums.Countries.Rus, Name = nameof(Common.Contracts.Constants.Enums.Countries.Rus), Info = "Russia" },
            new() { Id = (int) Common.Contracts.Constants.Enums.Countries.Usa, Name = nameof(Common.Contracts.Constants.Enums.Countries.Usa), Info = "USA" },
            new() { Id = (int) Common.Contracts.Constants.Enums.Countries.Chn, Name = nameof(Common.Contracts.Constants.Enums.Countries.Chn), Info = "China" },
            new() { Id = (int) Common.Contracts.Constants.Enums.Countries.Deu, Name = nameof(Common.Contracts.Constants.Enums.Countries.Deu), Info = "Deutschland" },
            new() { Id = (int) Common.Contracts.Constants.Enums.Countries.Gbr, Name = nameof(Common.Contracts.Constants.Enums.Countries.Gbr), Info = "Great Britain" },
            new() { Id = (int) Common.Contracts.Constants.Enums.Countries.Che, Name = nameof(Common.Contracts.Constants.Enums.Countries.Che), Info = "Switzerland" },
            new() { Id = (int) Common.Contracts.Constants.Enums.Countries.Jpn, Name = nameof(Common.Contracts.Constants.Enums.Countries.Jpn), Info = "Japan" }
        });
        builder.Entity<Exchange>().HasData(new Exchange[]
        {
            new() {Id = (int) Common.Contracts.Constants.Enums.Exchanges.Nasdaq, Name = nameof(Common.Contracts.Constants.Enums.Exchanges.Nasdaq) },
            new() {Id = (int) Common.Contracts.Constants.Enums.Exchanges.Nyse, Name = nameof(Common.Contracts.Constants.Enums.Exchanges.Nyse)},
            new() {Id = (int) Common.Contracts.Constants.Enums.Exchanges.Fwb, Name = nameof(Common.Contracts.Constants.Enums.Exchanges.Fwb)},
            new() {Id = (int) Common.Contracts.Constants.Enums.Exchanges.Hkse, Name = nameof(Common.Contracts.Constants.Enums.Exchanges.Hkse)},
            new() {Id = (int) Common.Contracts.Constants.Enums.Exchanges.Lse, Name = nameof(Common.Contracts.Constants.Enums.Exchanges.Lse)},
            new() {Id = (int) Common.Contracts.Constants.Enums.Exchanges.Sse, Name = nameof(Common.Contracts.Constants.Enums.Exchanges.Sse)},
            new() {Id = (int) Common.Contracts.Constants.Enums.Exchanges.Spbex, Name = nameof(Common.Contracts.Constants.Enums.Exchanges.Spbex)},
            new() {Id = (int) Common.Contracts.Constants.Enums.Exchanges.Moex, Name = nameof(Common.Contracts.Constants.Enums.Exchanges.Moex)},
            new() {Id = (int) Common.Contracts.Constants.Enums.Exchanges.Binance, Name = nameof(Common.Contracts.Constants.Enums.Exchanges.Binance)},
            new() {Id = (int) Common.Contracts.Constants.Enums.Exchanges.Ftx2, Name = nameof(Common.Contracts.Constants.Enums.Exchanges.Ftx2)},
            new() {Id = (int) Common.Contracts.Constants.Enums.Exchanges.Coinbase, Name = nameof(Common.Contracts.Constants.Enums.Exchanges.Coinbase)}
        });
        builder.Entity<OperationType>().HasData(new OperationType[]
        {
            new() {Id = (int)Core.Constants.Enums.OperationTypes.Income, Name = nameof(Core.Constants.Enums.OperationTypes.Income) },
            new() {Id = (int)Core.Constants.Enums.OperationTypes.Expense, Name = nameof(Core.Constants.Enums.OperationTypes.Expense) }
        });
        builder.Entity<Provider>().HasData(new Provider[]
        {
            new() {Id = (int)Core.Constants.Enums.Providers.Safe, Name = nameof(Core.Constants.Enums.Providers.Safe), Info = "Private storage" },
            new() {Id = (int)Core.Constants.Enums.Providers.Bcs, Name = nameof(Core.Constants.Enums.Providers.Bcs), Info = "Broker/Bank" },
            new() {Id = (int)Core.Constants.Enums.Providers.Tinkoff, Name = nameof(Core.Constants.Enums.Providers.Tinkoff), Info = "Broker/Bank" },
            new() {Id = (int)Core.Constants.Enums.Providers.Vtb, Name = nameof(Core.Constants.Enums.Providers.Vtb), Info = "Broker/Bank" },
            new() {Id = (int)Core.Constants.Enums.Providers.Bitokk, Name = nameof(Core.Constants.Enums.Providers.Bitokk), Info = "Сrypto exchange https://bitokk.biz/" },
            new() {Id = (int)Core.Constants.Enums.Providers.XChange, Name = nameof(Core.Constants.Enums.Providers.XChange), Info = "Сrypto exchange https://xchange.cash/" },
            new() {Id = (int)Core.Constants.Enums.Providers.JetLend, Name = nameof(Core.Constants.Enums.Providers.JetLend), Info = "Crowdlending platform https://jetlend.ru/" }
        });
        builder.Entity<EventType>().HasData(new EventType[]
        {
            new()
            {
                Id = (int)Core.Constants.Enums.EventTypes.Adding,
                Name = nameof(Core.Constants.Enums.EventTypes.Adding),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Income,
                Info = "Increasing the asset balance"
            },
            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.Withdrawing,
                Name = nameof(Core.Constants.Enums.EventTypes.Withdrawing),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
                Info = "Decreasing the asset balance"
            },

            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.BankInvestments,
                Name= nameof(Core.Constants.Enums.EventTypes.BankInvestments),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
                Info = "Investing in bank products"
            },
            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.CrowdfundingInvestments,
                Name= nameof(Core.Constants.Enums.EventTypes.CrowdfundingInvestments),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
                Info = "Investing in crowdfunding"
            },
            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.CrowdlendingInvestments,
                Name= nameof(Core.Constants.Enums.EventTypes.CrowdlendingInvestments),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
                Info = "Investing in crowdlending"
            },
            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.VentureInvestments,
                Name= nameof(Core.Constants.Enums.EventTypes.VentureInvestments),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
                Info = "Venture investments"
            },

            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.InterestProfit,
                Name= nameof(Core.Constants.Enums.EventTypes.InterestProfit),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Income,
                Info = "Interest profit"
            },
            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.InvestmentProfit,
                Name= nameof(Core.Constants.Enums.EventTypes.InvestmentProfit),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Income,
                Info = "Investment profit"
            },
            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.InvestmentBody,
                Name= nameof(Core.Constants.Enums.EventTypes.InvestmentBody),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Income,
                Info = "Returning of investment body"
            },

            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.Splitting,
                Name= nameof(Core.Constants.Enums.EventTypes.Splitting),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Income,
                Info = "Increasing an asset by dividing it"
            },

            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.Donation,
                Name= nameof(Core.Constants.Enums.EventTypes.Donation),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Income,
                Info = "Increasing an asset by donation it"
            },

            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.Coupon,
                Name= nameof(Core.Constants.Enums.EventTypes.Coupon),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Income,
                Info = "Coupons from assets"
            },
            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.Dividend,
                Name= nameof(Core.Constants.Enums.EventTypes.Dividend),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Income,
                Info = "Dividends from assets"
            },

            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.Delisting,
                Name= nameof(Core.Constants.Enums.EventTypes.Delisting),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
                Info = "Excluding an asset from lists"
            },
            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.Loss,
                Name= nameof(Core.Constants.Enums.EventTypes.Loss),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
                Info = "Loss registration"
            },
            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.TaxIncome,
                Name= nameof(Core.Constants.Enums.EventTypes.TaxIncome),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
                Info = "Tax on profit by an asset"
            },
            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.TaxCountry,
                Name= nameof(Core.Constants.Enums.EventTypes.TaxCountry),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
                Info = "Internal tax of country"
            },
            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.TaxDeal,
                Name= nameof(Core.Constants.Enums.EventTypes.TaxDeal),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
                Info = "Tax for a deal"
            },
            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.TaxProvider,
                Name= nameof(Core.Constants.Enums.EventTypes.TaxProvider),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
                Info = "Tax to provider"
            },
            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.TaxDepositary,
                Name= nameof(Core.Constants.Enums.EventTypes.TaxDepositary),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
                Info = "Tax to depositary of an asset"
            },
            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.ComissionDeal,
                Name= nameof(Core.Constants.Enums.EventTypes.ComissionDeal),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
                Info = "Commission for a deal"
            },
            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.ComissionProvider,
                Name= nameof(Core.Constants.Enums.EventTypes.ComissionProvider),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
                Info = "Commission to provider"
            },
            new()
            {
                Id= (int)Core.Constants.Enums.EventTypes.ComissionDepositary,
                Name= nameof(Core.Constants.Enums.EventTypes.ComissionDepositary),
                OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
                Info = "Commission to depositary of an asset"
            } });
        #endregion
    }
}