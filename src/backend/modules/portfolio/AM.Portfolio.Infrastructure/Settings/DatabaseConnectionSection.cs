using Net.Shared.Persistence.Models.Settings.Connections;

namespace AM.Portfolio.Infrastructure.Settings;

public sealed class DatabaseConnectionSection
{
    public const string Name = "DatabaseConnections";
    public PostgreSqlConnection PostgreSql { get; set; } = null!;
    public MongoDbConnection MongoDb { get; set; } = null!;
}
