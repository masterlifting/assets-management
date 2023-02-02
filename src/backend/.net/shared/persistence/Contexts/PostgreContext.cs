using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistence.Abstractions.Contexts;
using Shared.Persistence.Settings.Connections;

namespace Shared.Persistence.Contexts;

public abstract class PostgreContext : DbContext, IPostgrePersistenceContext
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly PostgreSQLConnectionSettings _connectionSettings;
    protected PostgreContext(ILoggerFactory loggerFactory, PostgreSQLConnectionSettings connectionSettings) : base()
    {
        _loggerFactory = loggerFactory;
        _connectionSettings = connectionSettings;
    }

    public Task CommitTransactionAsync(CancellationToken cToken)
    {
        throw new NotImplementedException();
    }

    public Task RollbackTransactionAsync(CancellationToken cToken)
    {
        throw new NotImplementedException();
    }

    public Task SetTransactionAsync(CancellationToken cToken)
    {
        throw new NotImplementedException();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseLoggerFactory(_loggerFactory);
        builder.UseNpgsql(_connectionSettings.GetConnectionString());
        base.OnConfiguring(builder);
    }
}
