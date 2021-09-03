
using IM.Services.Companies.Prices.Api.DataAccess.Entities;

using Microsoft.EntityFrameworkCore;

using static CommonServices.CommonEnums;

namespace IM.Services.Companies.Prices.Api.DataAccess
{
    public class PricesContext : DbContext
    {
        public DbSet<Ticker> Tickers { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<SourceType> SourceTypes { get; set; }

        public PricesContext(DbContextOptions<PricesContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Price>().HasKey(x => new { x.TickerName, x.Date });
            modelBuilder.Entity<Price>().HasOne(x => x.Ticker).WithMany(x => x.Prices).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SourceType>().Property(x => x.Id).ValueGeneratedNever();
            modelBuilder.Entity<SourceType>().HasData(new SourceType[]
            {
                new (){Id = (byte)PriceSourceTypes.MOEX, Name = nameof(PriceSourceTypes.MOEX) },
                new (){Id = (byte)PriceSourceTypes.Tdameritrade, Name = nameof(PriceSourceTypes.Tdameritrade) }
            });
        }
    }
}