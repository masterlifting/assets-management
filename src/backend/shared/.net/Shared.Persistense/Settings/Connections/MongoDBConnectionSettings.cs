namespace Shared.Persistense.Settings.Connections;

public sealed record MongoDBConnectionSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 27017;
    public string Database { get; set; } = null!;
    public string User { get; set; } = "root";
    public string Password { get; set; } = null!;

    public string GetConnectionString() => $"mongodb://{User}:{Password}@{Host}:{Port}/?authMechanism=DEFAULT";
}