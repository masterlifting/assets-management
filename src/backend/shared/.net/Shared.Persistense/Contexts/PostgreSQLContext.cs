using Microsoft.EntityFrameworkCore;

using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Settings.Connections;

namespace Shared.Persistense.Contexts;

public abstract class PostgreSQLContext : DbContext
{
    private readonly PostgreSQLConnectionSettings _connectionSettings;
    protected PostgreSQLContext(PostgreSQLConnectionSettings connectionSettings) : base() => 
        _connectionSettings = connectionSettings;

    public DbSet<GuidId> GuidIds { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionSettings.GetConnectionString());
        base.OnConfiguring(optionsBuilder);
    }
}
