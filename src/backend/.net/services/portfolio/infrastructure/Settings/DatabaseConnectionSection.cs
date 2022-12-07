using Shared.Persistence.Settings.Connections;

namespace AM.Services.Portfolio.Infrastructure.Settings;

public sealed class DatabaseConnectionSection
{
    public const string Name = "DatabaseConnections";
    public PostgreSQLConnectionSettings PostgreSQL { get; set; } = null!;
    public MongoDBConnectionSettings MongoDB { get; set; } = null!;
}