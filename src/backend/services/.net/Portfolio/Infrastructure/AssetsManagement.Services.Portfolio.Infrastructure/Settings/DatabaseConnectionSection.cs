using Shared.Infrastructure.Persistense.ConnectionSettings;

namespace AM.Services.Portfolio.Infrastructure.Settings;

public class DatabaseConnectionSection
{
    public const string Name = "DatabaseConnections";
    public PostgresConnectionSettings Postgres { get; set; } = null!;
}