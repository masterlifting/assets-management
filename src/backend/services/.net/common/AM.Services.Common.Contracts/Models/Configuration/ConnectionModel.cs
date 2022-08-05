namespace AM.Services.Common.Contracts.Models.Configuration;

public class ConnectionModel
{
    public string Server { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string Password { get; set; } = null!;
}