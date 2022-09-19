using Shared.Web.Connections;

namespace AM.Services.Portfolio.Infrastructure.Settings;

public class WebclientConnectionSection
{
    public const string Name = "WebclientConnections";
    public WebClientConnectionSettings Moex { get; set; } = null!;
}