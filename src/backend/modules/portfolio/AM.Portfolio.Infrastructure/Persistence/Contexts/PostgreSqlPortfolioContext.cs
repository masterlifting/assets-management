using AM.Portfolio.Core.Persistence.Entities.Sql;
using AM.Portfolio.Core.Persistence.Entities.Sql.Catalogs;
using AM.Portfolio.Infrastructure.Settings;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Net.Shared.Persistence.Contexts;

using CoreEnums = AM.Portfolio.Core.Constants.Enums;
using PersistenceEnums = Net.Shared.Persistence.Models.Constants.Enums;
using SharedEnums = AM.Shared.Models.Constants.Enums;

namespace AM.Portfolio.Infrastructure.Persistence.Contexts;

public sealed class PostgreSqlPortfolioContext : PostgreSqlContext
{
    #region Catalogs
    public DbSet<AssetType> AssetTypes { get; set; } = null!;
    public DbSet<EventType> EventTypes { get; set; } = null!;
    public DbSet<Zone> Zones { get; set; } = null!;
    public DbSet<Exchange> Exchanges { get; set; } = null!;
    public DbSet<Holder> Holders { get; set; } = null!;
    public DbSet<ProcessStatus> ProcessStatuses { get; set; } = null!;
    public DbSet<ProcessStep> ProcessSteps { get; set; } = null!;
    #endregion

    #region Entities
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<Asset> Assets { get; set; } = null!;
    public DbSet<Derivative> Derivatives { get; set; } = null!;
    public DbSet<Deal> Deals { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<Income> Incomes { get; set; } = null!;
    public DbSet<Expense> Expenses { get; set; } = null!;
    public DbSet<Balance> Balances { get; set; } = null!;
    #endregion

    public PostgreSqlPortfolioContext(ILoggerFactory loggerFactory, IOptions<DatabaseConnectionSection> options) : base(loggerFactory, options.Value.PostgreSql)
    {
        //Database.EnsureDeleted();
        //Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        const int DecimalPrecision = 18;
        const int DecimalScale = 10;
        const int ErrorMaxLength = 500;
        const int DescriptionMaxLength = 500;

        builder.UseSerialColumns();

        #region CONFIGURATIONS

        builder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Description).HasMaxLength(DescriptionMaxLength);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
        });
        builder.Entity<Account>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Agreement).IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).HasMaxLength(DescriptionMaxLength);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);

            e.HasIndex(x => new { x.Agreement, x.UserId, x.HolderId }).IsUnique();
        });

        builder.Entity<Asset>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Label).IsRequired().HasMaxLength(20);
            e.Property(x => x.Error).HasMaxLength(ErrorMaxLength);
            e.Property(x => x.Description).HasMaxLength(DescriptionMaxLength);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
            e.Property(x => x.Updated).HasDefaultValue(DateTime.UtcNow);

            e.HasIndex(x => new { x.Name, x.TypeId }).IsUnique();
        });
        builder.Entity<Derivative>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Ticker).IsRequired().HasMaxLength(50);
            e.Property(x => x.Error).HasMaxLength(ErrorMaxLength);
            e.Property(x => x.Description).HasMaxLength(DescriptionMaxLength);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
            e.Property(x => x.Updated).HasDefaultValue(DateTime.UtcNow);

            e.HasIndex(x => new { x.Ticker, x.AssetId }).IsUnique();
        });

        builder.Entity<Event>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Value).HasPrecision(DecimalPrecision, DecimalScale);
            e.Property(x => x.Error).HasMaxLength(ErrorMaxLength);
            e.Property(x => x.Description).HasMaxLength(DescriptionMaxLength);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
            e.Property(x => x.Updated).HasDefaultValue(DateTime.UtcNow);
        });
        builder.Entity<Deal>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Error).HasMaxLength(ErrorMaxLength);
            e.Property(x => x.Description).HasMaxLength(DescriptionMaxLength);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
            e.Property(x => x.Updated).HasDefaultValue(DateTime.UtcNow);
        });
        builder.Entity<Income>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Value).HasPrecision(DecimalPrecision, DecimalScale);
            e.Property(x => x.Description).HasMaxLength(DescriptionMaxLength);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
        });
        builder.Entity<Expense>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Value).HasPrecision(DecimalPrecision, DecimalScale);
            e.Property(x => x.Description).HasMaxLength(DescriptionMaxLength);
            e.Property(x => x.Created).HasDefaultValue(DateTime.UtcNow);
        });
        builder.Entity<Balance>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Value).HasPrecision(DecimalPrecision, DecimalScale);
            e.Property(x => x.Description).HasMaxLength(DescriptionMaxLength);
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
        builder.Entity<EventType>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            e.HasIndex(x => new { x.Name, x.IsIncreasable }).IsUnique();
        });

        builder.Entity<Zone>(e =>
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
        builder.Entity<Holder>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.Name).IsUnique();
        });

        #endregion

        #region CREATE DATA

        builder.Entity<ProcessStep>().HasData(new ProcessStep[]
        {
            new(){Id = (int)CoreEnums.ProcessSteps.None, Name = nameof(CoreEnums.ProcessSteps.None)},
            new(){Id = (int)CoreEnums.ProcessSteps.CalculateSplitting, Name = nameof(CoreEnums.ProcessSteps.CalculateSplitting)},
            new(){Id = (int)CoreEnums.ProcessSteps.CalculateBalance, Name = nameof(CoreEnums.ProcessSteps.CalculateBalance)},
            new(){Id = (int)CoreEnums.ProcessSteps.ParseBcsCompanies, Name = nameof(CoreEnums.ProcessSteps.ParseBcsCompanies)},
            new(){Id = (int)CoreEnums.ProcessSteps.ParseBcsTransactions, Name = nameof(CoreEnums.ProcessSteps.ParseBcsTransactions)},
            new(){Id = (int)CoreEnums.ProcessSteps.ParseRaiffeisenSrbTransactions, Name = nameof(CoreEnums.ProcessSteps.ParseRaiffeisenSrbTransactions)},
        });
        builder.Entity<Holder>().HasData(new Holder[]
        {
            new() {Id = (int)CoreEnums.Holders.Cash, Name = nameof(CoreEnums.Holders.Cash), Description = "Private storage" },
            new() {Id = (int)CoreEnums.Holders.LedgerNanoX, Name = nameof(CoreEnums.Holders.LedgerNanoX), Description = "Crypto wallet" },
            new() {Id = (int)CoreEnums.Holders.Bcs, Name = nameof(CoreEnums.Holders.Bcs), Description = "Russian Broker, Bank" },
            new() {Id = (int)CoreEnums.Holders.Vtb, Name = nameof(CoreEnums.Holders.Vtb), Description = "Russian Broker, Bank" },
            new() {Id = (int)CoreEnums.Holders.JetLend, Name = nameof(CoreEnums.Holders.JetLend), Description = "Russian Crowdlending platform https://jetlend.ru/" },
            new() {Id = (int)CoreEnums.Holders.Qiwi, Name = nameof(CoreEnums.Holders.Qiwi), Description = "Russian Bank, Wallet" },
            new() {Id = (int)CoreEnums.Holders.RaiffeisenRussia, Name = nameof(CoreEnums.Holders.RaiffeisenRussia), Description = "Russian Bank, Broker" },
            new() {Id = (int)CoreEnums.Holders.RaiffeisenSerbia, Name = nameof(CoreEnums.Holders.RaiffeisenSerbia), Description = "Serbian Bank, Broker" },
            new() {Id = (int)CoreEnums.Holders.PostanskaStedionica, Name = nameof(CoreEnums.Holders.PostanskaStedionica), Description = "Serbian Bank" },
            new() {Id = (int)CoreEnums.Holders.ExpobankSerbia, Name = nameof(CoreEnums.Holders.ExpobankSerbia), Description = "Serbian Bank" }
        });

        builder.Entity<Zone>().HasData(new Zone[]
        {
            new() { Id = (int) SharedEnums.Zones.World, Name = nameof(SharedEnums.Zones.World), Description = "World" },
            new() { Id = (int) SharedEnums.Zones.Eu, Name = nameof(SharedEnums.Zones.Eu), Description = "Europe" },
            new() { Id = (int) SharedEnums.Zones.Rus, Name = nameof(SharedEnums.Zones.Rus), Description = "Russia" },
            new() { Id = (int) SharedEnums.Zones.Usa, Name = nameof(SharedEnums.Zones.Usa), Description = "USA" },
            new() { Id = (int) SharedEnums.Zones.Chn, Name = nameof(SharedEnums.Zones.Chn), Description = "China" },
            new() { Id = (int) SharedEnums.Zones.Deu, Name = nameof(SharedEnums.Zones.Deu), Description = "Deutschland" },
            new() { Id = (int) SharedEnums.Zones.Gbr, Name = nameof(SharedEnums.Zones.Gbr), Description = "Great Britain" },
            new() { Id = (int) SharedEnums.Zones.Che, Name = nameof(SharedEnums.Zones.Che), Description = "Switzerland" },
            new() { Id = (int) SharedEnums.Zones.Jpn, Name = nameof(SharedEnums.Zones.Jpn), Description = "Japan" },
            new() { Id = (int) SharedEnums.Zones.Srb, Name = nameof(SharedEnums.Zones.Srb), Description = "Serbia" },
            new() { Id = (int) SharedEnums.Zones.Pan, Name = nameof(SharedEnums.Zones.Pan), Description = "Panama" },
            new() { Id = (int) SharedEnums.Zones.Nld, Name = nameof(SharedEnums.Zones.Nld), Description = "Netherlands" },
            new() { Id = (int) SharedEnums.Zones.Irl, Name = nameof(SharedEnums.Zones.Irl), Description = "Ireland" },
            new() { Id = (int) SharedEnums.Zones.Lbr, Name = nameof(SharedEnums.Zones.Lbr), Description = "Liberia" },
            new() { Id = (int) SharedEnums.Zones.Jey, Name = nameof(SharedEnums.Zones.Jey), Description = "Jersey" },
        });
        builder.Entity<Exchange>().HasData(new Exchange[]
        {
            new() {Id = (int) SharedEnums.Exchanges.P2P, Name = nameof(SharedEnums.Exchanges.P2P) },
            new() {Id = (int) SharedEnums.Exchanges.Nasdaq, Name = nameof(SharedEnums.Exchanges.Nasdaq) },
            new() {Id = (int) SharedEnums.Exchanges.Nyse, Name = nameof(SharedEnums.Exchanges.Nyse)},
            new() {Id = (int) SharedEnums.Exchanges.Fwb, Name = nameof(SharedEnums.Exchanges.Fwb)},
            new() {Id = (int) SharedEnums.Exchanges.Hkse, Name = nameof(SharedEnums.Exchanges.Hkse)},
            new() {Id = (int) SharedEnums.Exchanges.Lse, Name = nameof(SharedEnums.Exchanges.Lse)},
            new() {Id = (int) SharedEnums.Exchanges.Sse, Name = nameof(SharedEnums.Exchanges.Sse)},
            new() {Id = (int) SharedEnums.Exchanges.Spbex, Name = nameof(SharedEnums.Exchanges.Spbex)},
            new() {Id = (int) SharedEnums.Exchanges.Moex, Name = nameof(SharedEnums.Exchanges.Moex)},
            new() {Id = (int) SharedEnums.Exchanges.Binance, Name = nameof(SharedEnums.Exchanges.Binance)},
            new() {Id = (int) SharedEnums.Exchanges.Ftx2, Name = nameof(SharedEnums.Exchanges.Ftx2)},
            new() {Id = (int) SharedEnums.Exchanges.Coinbase, Name = nameof(SharedEnums.Exchanges.Coinbase)},
            new() {Id = (int) SharedEnums.Exchanges.Bitokk, Name = nameof(SharedEnums.Exchanges.Bitokk), Description = "Crypto exchange https://bitokk.biz/" },
            new() {Id = (int) SharedEnums.Exchanges.XChange, Name = nameof(SharedEnums.Exchanges.XChange), Description = "Crypto exchange https://xchange.cash/" },
        });

        builder.Entity<ProcessStatus>().HasData(new ProcessStatus[]
        {
            new(){Id = (int)PersistenceEnums.ProcessStatuses.Draft, Name = nameof(PersistenceEnums.ProcessStatuses.Draft), Description = "Draft" },
            new(){Id = (int)PersistenceEnums.ProcessStatuses.Ready, Name = nameof(PersistenceEnums.ProcessStatuses.Ready), Description = "Ready to processing data" },
            new(){Id = (int)PersistenceEnums.ProcessStatuses.Processing, Name = nameof(PersistenceEnums.ProcessStatuses.Processing), Description = "Processing data" },
            new(){Id = (int)PersistenceEnums.ProcessStatuses.Processed, Name = nameof(PersistenceEnums.ProcessStatuses.Processed), Description = "Processed step" },
            new(){Id = (int)PersistenceEnums.ProcessStatuses.Completed, Name = nameof(PersistenceEnums.ProcessStatuses.Completed), Description = "Processed task" },
            new(){Id = (int)PersistenceEnums.ProcessStatuses.Error, Name = nameof(PersistenceEnums.ProcessStatuses.Error), Description = "Error of processing" }
        });

        builder.Entity<EventType>().HasData(new EventType[]
        {
            new() { Id= 1, Name= nameof(CoreEnums.EventTypes.Percentage), IsIncreasable = true },
            new() { Id= 2, Name= nameof(CoreEnums.EventTypes.Donation), IsIncreasable = true },
            new() { Id= 3, Name= nameof(CoreEnums.EventTypes.Donation), IsIncreasable = false },
            new() { Id= 4, Name= nameof(CoreEnums.EventTypes.Tax), IsIncreasable = false },
            new() { Id= 5, Name= nameof(CoreEnums.EventTypes.Commission), IsIncreasable = false },
            new() { Id= 6, Name= nameof(CoreEnums.EventTypes.Bankruptcy), IsIncreasable = false },
            new() { Id= 7, Name= nameof(CoreEnums.EventTypes.Fail), IsIncreasable = false },
            new() { Id= 8, Name= nameof(CoreEnums.EventTypes.Splitting), IsIncreasable = true },
            new() { Id= 9, Name= nameof(CoreEnums.EventTypes.Multiplying), IsIncreasable = true },
            new() { Id= 10, Name= nameof(CoreEnums.EventTypes.TopUp), IsIncreasable = true },
            new() { Id= 11, Name= nameof(CoreEnums.EventTypes.Withdraw), IsIncreasable = false },
        });

        builder.Entity<AssetType>().HasData(new AssetType[]
        {
            new() {Id = (int) SharedEnums.AssetTypes.Things, Name = nameof(SharedEnums.AssetTypes.Things), Description = "Different things" },
            new() {Id = (int) SharedEnums.AssetTypes.Stock, Name = nameof(SharedEnums.AssetTypes.Stock), Description = "Stocks" },
            new() {Id = (int) SharedEnums.AssetTypes.Bond, Name = nameof(SharedEnums.AssetTypes.Bond), Description = "Bonds" },
            new() {Id = (int) SharedEnums.AssetTypes.Fund, Name = nameof(SharedEnums.AssetTypes.Fund), Description = "Founds" },
            new() {Id = (int) SharedEnums.AssetTypes.Fiat, Name = nameof(SharedEnums.AssetTypes.Fiat), Description = "Fiat currencies" },
            new() {Id = (int) SharedEnums.AssetTypes.Crypto, Name = nameof(SharedEnums.AssetTypes.Crypto), Description = "Crypto currencies" },
            new() {Id = (int) SharedEnums.AssetTypes.NFT, Name = nameof(SharedEnums.AssetTypes.NFT), Description = "NFT tokens"},
            new() {Id = (int) SharedEnums.AssetTypes.Estate, Name = nameof(SharedEnums.AssetTypes.Estate), Description = "Estates"},
        });
        builder.Entity<Asset>().HasData(new Asset[]
        {
            new()
            {
                Id = 1,
                Name = Shared.Models.Constants.Assets.Eur,
                TypeId = (int)SharedEnums.AssetTypes.Fiat,
                Label = "€",
                StepId = (int)CoreEnums.ProcessSteps.None,
                StatusId = (int)PersistenceEnums.ProcessStatuses.Draft,
            },
            new()
            {
                Id = 2,
                Name = Shared.Models.Constants.Assets.Rub,
                TypeId = (int)SharedEnums.AssetTypes.Fiat,
                Label = "₽",
                StepId = (int)CoreEnums.ProcessSteps.None,
                StatusId = (int)PersistenceEnums.ProcessStatuses.Draft,
            },
            new()
            {
                Id = 3,
                Name = Shared.Models.Constants.Assets.Usd,
                TypeId = (int)SharedEnums.AssetTypes.Fiat,
                Label = "$",
                StepId = (int)CoreEnums.ProcessSteps.None,
                StatusId = (int)PersistenceEnums.ProcessStatuses.Draft,
            },
            new()
            {
                Id = 4,
                Name = Shared.Models.Constants.Assets.Gbp,
                TypeId = (int)SharedEnums.AssetTypes.Fiat,
                Label = "£",
                StepId = (int)CoreEnums.ProcessSteps.None,
                StatusId = (int)PersistenceEnums.ProcessStatuses.Draft,
            },
            new()
            {
                Id = 5,
                Name = Shared.Models.Constants.Assets.Chy,
                TypeId = (int)SharedEnums.AssetTypes.Fiat,
                Label = "¥",
                StepId = (int)CoreEnums.ProcessSteps.None,
                StatusId = (int)PersistenceEnums.ProcessStatuses.Draft,
            },
            new()
            {
                Id = 6,
                Name = Shared.Models.Constants.Assets.Rsd,
                TypeId = (int)SharedEnums.AssetTypes.Fiat,
                Label = "din.",
                StepId = (int)CoreEnums.ProcessSteps.None,
                StatusId = (int)PersistenceEnums.ProcessStatuses.Draft,
            },
            new()
            {
                Id = 7,
                Name = Shared.Models.Constants.Assets.Btc,
                TypeId = (int)SharedEnums.AssetTypes.Crypto,
                Label = "₿",
                StepId = (int)CoreEnums.ProcessSteps.None,
                StatusId = (int)PersistenceEnums.ProcessStatuses.Draft,
            },
            new()
            {
                Id = 8,
                Name = Shared.Models.Constants.Assets.Eth,
                TypeId = (int)SharedEnums.AssetTypes.Crypto,
                Label = "Ξ",
                StepId = (int)CoreEnums.ProcessSteps.None,
                StatusId = (int)PersistenceEnums.ProcessStatuses.Draft,
            }
        });

        builder.Entity<Derivative>().HasData(new Derivative[]
        {
            new()
            {
                Id = 1,
                AssetId = 1,
                Ticker = "EUR",
                ZoneId = (int)SharedEnums.Zones.Eu,
                StepId = (int)CoreEnums.ProcessSteps.None,
                StatusId = (int)PersistenceEnums.ProcessStatuses.Draft,
                Description = Shared.Models.Constants.Assets.Eur,
            },
            new()
            {
                Id = 2,
                AssetId = 2,
                Ticker = "RUB",
                ZoneId = (int)SharedEnums.Zones.Rus,
                StepId = (int)CoreEnums.ProcessSteps.None,
                StatusId = (int)PersistenceEnums.ProcessStatuses.Draft,
                Description = Shared.Models.Constants.Assets.Rub
            },
            new()
            {
                Id = 3,
                AssetId = 3,
                Ticker = "USD",
                ZoneId = (int)SharedEnums.Zones.Usa,
                StepId = (int)CoreEnums.ProcessSteps.None,
                StatusId = (int)PersistenceEnums.ProcessStatuses.Draft,
                Description = Shared.Models.Constants.Assets.Usd
            },
            new()
            {
                Id = 4,
                AssetId = 4,
                Ticker = "GBP",
                ZoneId = (int)SharedEnums.Zones.Gbr,
                StepId = (int)CoreEnums.ProcessSteps.None,
                StatusId = (int)PersistenceEnums.ProcessStatuses.Draft,
                Description = Shared.Models.Constants.Assets.Gbp
            },
            new()
            {
                Id = 5,
                AssetId = 5,
                Ticker = "CNY",
                ZoneId = (int)SharedEnums.Zones.Chn,
                StepId = (int)CoreEnums.ProcessSteps.None,
                StatusId = (int)PersistenceEnums.ProcessStatuses.Draft,
                Description = Shared.Models.Constants.Assets.Chy
            },
            new()
            {
                Id = 6,
                AssetId = 6,
                Ticker = "RSD",
                ZoneId = (int)SharedEnums.Zones.Srb,
                StepId = (int)CoreEnums.ProcessSteps.None,
                StatusId = (int)PersistenceEnums.ProcessStatuses.Draft,
                Description = Shared.Models.Constants.Assets.Rsd
            },
            new()
            {
                Id = 7,
                AssetId = 7,
                Ticker = "BTC",
                ZoneId = (int)SharedEnums.Zones.World,
                StepId = (int)CoreEnums.ProcessSteps.None,
                StatusId = (int)PersistenceEnums.ProcessStatuses.Draft,
                Description = Shared.Models.Constants.Assets.Btc
            },
            new()
            {
                Id = 8,
                AssetId = 8,
                Ticker = "ETH",
                ZoneId = (int)SharedEnums.Zones.World,
                StepId = (int)CoreEnums.ProcessSteps.None,
                StatusId = (int)PersistenceEnums.ProcessStatuses.Draft,
                Description = Shared.Models.Constants.Assets.Eth
            }
        });

        #endregion

        #region CREATE PERSONAL DATA

        builder.Entity<User>().HasData(new User
        {
            Id = 1,
            Name = "Andrei Pestunov",
            Description = "Owner of the application"
        });
        builder.Entity<Account>().HasData(new Account[]
        {
            new()
            {
                Id = 1,
                UserId = 1,
                HolderId = (int)CoreEnums.Holders.Bcs,
                Agreement = "332510/17-м от 14.08.2017",
                Description = "BCS Broker main account."
            },
            new()
            {
                Id = 2,
                UserId = 1,
                HolderId = (int)CoreEnums.Holders.Bcs,
                Agreement = "472746/18-м-иис от 17.07.2018",
                Description = "BCS Broker individual investment account."
            },
            new()
            {
                Id = 3,
                UserId = 1,
                HolderId = (int)CoreEnums.Holders.LedgerNanoX,
                Agreement = Shared.Models.Constants.Assets.Btc,
                Description = "Bitcoin wallet account."
            },
            new()
            {
                Id = 4,
                UserId = 1,
                HolderId = (int)CoreEnums.Holders.LedgerNanoX,
                Agreement = Shared.Models.Constants.Assets.Eth,
                Description = "Ethereum wallet account."
            },
            new()
            {
                Id = 5,
                UserId = 1,
                HolderId = (int)CoreEnums.Holders.JetLend,
                Agreement = "Jetlend",
                Description = "Jetlend crowdfunding platform account."
            },
            new()
            {
                Id = 6,
                UserId = 1,
                HolderId = (int)CoreEnums.Holders.RaiffeisenRussia,
                Agreement = "RaiffeisenRussia",
                Description = "Raiffeisen Russia Bank account."
            },
            new()
            {
                Id = 7,
                UserId = 1,
                HolderId = (int)CoreEnums.Holders.RaiffeisenSerbia,
                Agreement = "RaiffeisenSerbia",
                Description = "Raiffeisen Serbia Bank account."
            },
            new()
            {
                Id = 8,
                UserId = 1,
                HolderId = (int)CoreEnums.Holders.PostanskaStedionica,
                Agreement = "PostanskaStedionica",
                Description = "Postanska Stedionica Serbia Bank account."
            },
            new()
            {
                Id = 9,
                UserId = 1,
                HolderId = (int)CoreEnums.Holders.Cash,
                Agreement = "Cache",
                Description = "Cache safe."
            },
        });

        #endregion
    }
}
