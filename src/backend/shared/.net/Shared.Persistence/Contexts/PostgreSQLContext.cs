using Microsoft.EntityFrameworkCore;

using Shared.Persistence.Abstractions.Entities;
using Shared.Persistence.Settings.Connections;

namespace Shared.Persistence.Contexts;

public abstract class PostgreSQLContext : DbContext
{
    private readonly PostgreSQLConnectionSettings _connectionSettings;
    protected PostgreSQLContext(PostgreSQLConnectionSettings connectionSettings) : base() =>
        _connectionSettings = connectionSettings;

    public DbSet<GuidId> GuidIds { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseNpgsql(_connectionSettings.GetConnectionString());
        base.OnConfiguring(builder);
    }
}
