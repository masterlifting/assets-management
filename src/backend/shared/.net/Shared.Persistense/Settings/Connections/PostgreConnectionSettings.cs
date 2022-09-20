namespace Shared.Persistense.Settings.Connections;

public sealed class PostgreConnectionSettings
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Database { get; set; } = null!;
    public string User { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string GetConnectionString() => $"Server={Host};Port={Port};Database={Database};UserId={User};Password={Password}";
}