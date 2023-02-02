using AM.Services.Common;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;
using AM.Services.Portfolio.Infrastructure.Settings;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Shared.Persistence.Contexts;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Contexts;

public sealed class PostgrePortfolioContext : PostgreContext
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

    public PostgrePortfolioContext(ILoggerFactory loggerFactory, IOptions<DatabaseConnectionSection> options) : base(loggerFactory, options.Value.PostgreSQL)
    {
        //Database.EnsureDeleted();
        //Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.UseSerialColumns();

        builder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
        });
        builder.Entity<Account>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);

            e.HasIndex(x => new { x.Name, x.UserId, x.ProviderId }).IsUnique();
        });
        builder.Entity<Event>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Value).HasPrecision(18, 10);
            e.Property(x => x.Error).HasMaxLength(500);
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
            e.Property(x => x.Updated).HasDefaultValue(DateTime.UtcNow);
        });
        builder.Entity<Deal>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Cost).HasPrecision(18, 10);
            e.Property(x => x.Error).HasMaxLength(500);
            e.Property(x => x.Description).HasMaxLength(500);
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
            e.Property(x => x.Error).HasMaxLength(500);
            e.Property(x => x.Description).HasMaxLength(500);
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
            e.Property(x => x.Error).HasMaxLength(500);
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
            e.Property(x => x.Updated).HasDefaultValue(DateTime.UtcNow);

            e.HasIndex(x => new { x.Code, x.Name}).IsUnique();
        });
        builder.Entity<Income>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Value).HasPrecision(18, 10);
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
        });
        builder.Entity<Expense>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Value).HasPrecision(18, 10);
            e.Property(x => x.Description).HasMaxLength(500);
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
        new(){Id = (int)Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Draft, Name = nameof(Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Draft), Description = "Draft" },
        new(){Id = (int)Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Ready, Name = nameof(Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Ready), Description = "Ready to processing data" },
        new(){Id = (int)Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Processing, Name = nameof(Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Processing), Description = "Processing data" },
        new(){Id = (int)Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Processed, Name = nameof(Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Processed), Description = "Processed data" },
        new(){Id = (int)Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Error, Name = nameof(Shared.Persistence.Abstractions.Constants.Enums.ProcessStatuses.Error), Description = "Error of processing" }
        });
        builder.Entity<ProcessStep>().HasData(new ProcessStep[]
        {
        new(){Id = (int)Core.Constants.Enums.ProcessSteps.ParseBcsReport, Name = nameof(Core.Constants.Enums.ProcessSteps.ParseBcsReport)}
        });
        builder.Entity<AssetType>().HasData(new AssetType[]
        {
        new() {Id = (int) Constants.Enums.AssetTypes.Valuable, Name = nameof(Constants.Enums.AssetTypes.Valuable), Description = "Valuable" },
        new() {Id = (int) Constants.Enums.AssetTypes.Stock, Name = nameof(Constants.Enums.AssetTypes.Stock), Description = "Stocks" },
        new() {Id = (int) Constants.Enums.AssetTypes.Bond, Name = nameof(Constants.Enums.AssetTypes.Bond), Description = "Bonds" },
        new() {Id = (int) Constants.Enums.AssetTypes.Fund, Name = nameof(Constants.Enums.AssetTypes.Fund), Description = "Founds" },
        new() {Id = (int) Constants.Enums.AssetTypes.CurrencyFiat, Name = nameof(Constants.Enums.AssetTypes.CurrencyFiat), Description = "Fiat currencies" },
        new() {Id = (int) Constants.Enums.AssetTypes.CurrencyToken, Name = nameof(Constants.Enums.AssetTypes.CurrencyToken), Description = "Crypto currencies" },
        new() {Id = (int) Constants.Enums.AssetTypes.NftToken, Name = nameof(Constants.Enums.AssetTypes.NftToken), Description = "NFT tokens"},
        new() {Id = (int) Constants.Enums.AssetTypes.RealEstate, Name = nameof(Constants.Enums.AssetTypes.RealEstate), Description = "Real estates"},
        new() {Id = (int) Constants.Enums.AssetTypes.PersonalEstate, Name = nameof(Constants.Enums.AssetTypes.PersonalEstate), Description = "Personal estates"}
        });
        builder.Entity<Country>().HasData(new Country[]
        {
        new() { Id = (int) Constants.Enums.Countries.Rus, Name = nameof(Constants.Enums.Countries.Rus), Description = "Russia" },
        new() { Id = (int) Constants.Enums.Countries.Usa, Name = nameof(Constants.Enums.Countries.Usa), Description = "USA" },
        new() { Id = (int) Constants.Enums.Countries.Chn, Name = nameof(Constants.Enums.Countries.Chn), Description = "China" },
        new() { Id = (int) Constants.Enums.Countries.Deu, Name = nameof(Constants.Enums.Countries.Deu), Description = "Deutschland" },
        new() { Id = (int) Constants.Enums.Countries.Gbr, Name = nameof(Constants.Enums.Countries.Gbr), Description = "Great Britain" },
        new() { Id = (int) Constants.Enums.Countries.Che, Name = nameof(Constants.Enums.Countries.Che), Description = "Switzerland" },
        new() { Id = (int) Constants.Enums.Countries.Jpn, Name = nameof(Constants.Enums.Countries.Jpn), Description = "Japan" }
        });
        builder.Entity<Exchange>().HasData(new Exchange[]
        {
        new() {Id = (int) Constants.Enums.Exchanges.Nasdaq, Name = nameof(Constants.Enums.Exchanges.Nasdaq) },
        new() {Id = (int) Constants.Enums.Exchanges.Nyse, Name = nameof(Constants.Enums.Exchanges.Nyse)},
        new() {Id = (int) Constants.Enums.Exchanges.Fwb, Name = nameof(Constants.Enums.Exchanges.Fwb)},
        new() {Id = (int) Constants.Enums.Exchanges.Hkse, Name = nameof(Constants.Enums.Exchanges.Hkse)},
        new() {Id = (int) Constants.Enums.Exchanges.Lse, Name = nameof(Constants.Enums.Exchanges.Lse)},
        new() {Id = (int) Constants.Enums.Exchanges.Sse, Name = nameof(Constants.Enums.Exchanges.Sse)},
        new() {Id = (int) Constants.Enums.Exchanges.Spbex, Name = nameof(Constants.Enums.Exchanges.Spbex)},
        new() {Id = (int) Constants.Enums.Exchanges.Moex, Name = nameof(Constants.Enums.Exchanges.Moex)},
        new() {Id = (int) Constants.Enums.Exchanges.Binance, Name = nameof(Constants.Enums.Exchanges.Binance)},
        new() {Id = (int) Constants.Enums.Exchanges.Ftx2, Name = nameof(Constants.Enums.Exchanges.Ftx2)},
        new() {Id = (int) Constants.Enums.Exchanges.Coinbase, Name = nameof(Constants.Enums.Exchanges.Coinbase)}
        });
        builder.Entity<OperationType>().HasData(new OperationType[]
        {
        new() {Id = (int)Core.Constants.Enums.OperationTypes.Income, Name = nameof(Core.Constants.Enums.OperationTypes.Income) },
        new() {Id = (int)Core.Constants.Enums.OperationTypes.Expense, Name = nameof(Core.Constants.Enums.OperationTypes.Expense) }
        });
        builder.Entity<Provider>().HasData(new Provider[]
        {
        new() {Id = (int)Core.Constants.Enums.Providers.Safe, Name = nameof(Core.Constants.Enums.Providers.Safe), Description = "Private storage" },
        new() {Id = (int)Core.Constants.Enums.Providers.Bcs, Name = nameof(Core.Constants.Enums.Providers.Bcs), Description = "Broker/Bank" },
        new() {Id = (int)Core.Constants.Enums.Providers.Tinkoff, Name = nameof(Core.Constants.Enums.Providers.Tinkoff), Description = "Broker/Bank" },
        new() {Id = (int)Core.Constants.Enums.Providers.Vtb, Name = nameof(Core.Constants.Enums.Providers.Vtb), Description = "Broker/Bank" },
        new() {Id = (int)Core.Constants.Enums.Providers.Bitokk, Name = nameof(Core.Constants.Enums.Providers.Bitokk), Description = "Сrypto exchange https://bitokk.biz/" },
        new() {Id = (int)Core.Constants.Enums.Providers.XChange, Name = nameof(Core.Constants.Enums.Providers.XChange), Description = "Сrypto exchange https://xchange.cash/" },
        new() {Id = (int)Core.Constants.Enums.Providers.JetLend, Name = nameof(Core.Constants.Enums.Providers.JetLend), Description = "Crowdlending platform https://jetlend.ru/" }
        });
        builder.Entity<EventType>().HasData(new EventType[]
        {
        new()
        {
            Id = (int)Core.Constants.Enums.EventTypes.Adding,
            Name = nameof(Core.Constants.Enums.EventTypes.Adding),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Income,
            Description = "Increasing the asset balance"
        },
        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.Withdrawing,
            Name = nameof(Core.Constants.Enums.EventTypes.Withdrawing),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
            Description = "Decreasing the asset balance"
        },

        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.BankInvestments,
            Name= nameof(Core.Constants.Enums.EventTypes.BankInvestments),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
            Description = "Investing in bank products"
        },
        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.CrowdfundingInvestments,
            Name= nameof(Core.Constants.Enums.EventTypes.CrowdfundingInvestments),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
            Description = "Investing in crowdfunding"
        },
        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.CrowdlendingInvestments,
            Name= nameof(Core.Constants.Enums.EventTypes.CrowdlendingInvestments),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
            Description = "Investing in crowdlending"
        },
        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.VentureInvestments,
            Name= nameof(Core.Constants.Enums.EventTypes.VentureInvestments),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
            Description = "Venture investments"
        },

        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.InterestProfit,
            Name= nameof(Core.Constants.Enums.EventTypes.InterestProfit),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Income,
            Description = "Interest profit"
        },
        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.InvestmentProfit,
            Name= nameof(Core.Constants.Enums.EventTypes.InvestmentProfit),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Income,
            Description = "Investment profit"
        },
        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.InvestmentBody,
            Name= nameof(Core.Constants.Enums.EventTypes.InvestmentBody),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Income,
            Description = "Returning of investment body"
        },

        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.Splitting,
            Name= nameof(Core.Constants.Enums.EventTypes.Splitting),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Income,
            Description = "Increasing an asset by dividing it"
        },

        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.Donation,
            Name= nameof(Core.Constants.Enums.EventTypes.Donation),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Income,
            Description = "Increasing an asset by donation it"
        },

        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.Coupon,
            Name= nameof(Core.Constants.Enums.EventTypes.Coupon),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Income,
            Description = "Coupons from assets"
        },
        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.Dividend,
            Name= nameof(Core.Constants.Enums.EventTypes.Dividend),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Income,
            Description = "Dividends from assets"
        },

        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.Delisting,
            Name= nameof(Core.Constants.Enums.EventTypes.Delisting),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
            Description = "Excluding an asset from lists"
        },
        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.Loss,
            Name= nameof(Core.Constants.Enums.EventTypes.Loss),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
            Description = "Loss registration"
        },
        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.TaxIncome,
            Name= nameof(Core.Constants.Enums.EventTypes.TaxIncome),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
            Description = "Tax on profit by an asset"
        },
        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.TaxCountry,
            Name= nameof(Core.Constants.Enums.EventTypes.TaxCountry),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
            Description = "Internal tax of country"
        },
        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.TaxDeal,
            Name= nameof(Core.Constants.Enums.EventTypes.TaxDeal),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
            Description = "Tax for a deal"
        },
        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.TaxProvider,
            Name= nameof(Core.Constants.Enums.EventTypes.TaxProvider),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
            Description = "Tax to provider"
        },
        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.TaxDepositary,
            Name= nameof(Core.Constants.Enums.EventTypes.TaxDepositary),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
            Description = "Tax to depositary of an asset"
        },
        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.ComissionDeal,
            Name= nameof(Core.Constants.Enums.EventTypes.ComissionDeal),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
            Description = "Commission for a deal"
        },
        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.ComissionProvider,
            Name= nameof(Core.Constants.Enums.EventTypes.ComissionProvider),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
            Description = "Commission to provider"
        },
        new()
        {
            Id= (int)Core.Constants.Enums.EventTypes.ComissionDepositary,
            Name= nameof(Core.Constants.Enums.EventTypes.ComissionDepositary),
            OperationTypeId = (int)Core.Constants.Enums.OperationTypes.Expense,
            Description = "Commission to depositary of an asset"
        } });
        #endregion
    }
}
