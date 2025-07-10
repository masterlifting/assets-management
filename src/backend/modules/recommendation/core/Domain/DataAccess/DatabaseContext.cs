using AM.Services.Common.Contracts.Models.Entity;
using AM.Services.Recommendations.Domain.Entities;
using AM.Services.Recommendations.Domain.Entities.Catalogs;
using Microsoft.EntityFrameworkCore;

namespace AM.Services.Recommendations.Domain.DataAccess;

public class DatabaseContext : DbContext
{
    public DbSet<AssetType> AssetTypes { get; set; } = null!;
    public DbSet<Asset> Assets { get; set; } = null!;
    public DbSet<Purchase> Purchases { get; set; } = null!;
    public DbSet<Sale> Sales { get; set; } = null!;

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        //Database.EnsureDeleted();
        //Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asset>().HasKey(x => new { x.Id, x.TypeId });
        modelBuilder.Entity<AssetType>().HasData(Catalogs.AssetTypes);
        modelBuilder.Entity<Country>().HasData(Catalogs.Countries);

        modelBuilder.Entity<Sale>().HasIndex(x => new { x.AssetId, x.AssetTypeId });
        modelBuilder.Entity<Purchase>().HasIndex(x => new { x.AssetId, x.AssetTypeId });
    }
}