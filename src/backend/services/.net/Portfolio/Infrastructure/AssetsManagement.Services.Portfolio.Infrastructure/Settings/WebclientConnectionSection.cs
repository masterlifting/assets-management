using Shared.Web.Settings.Connections;

namespace AM.Services.Portfolio.Infrastructure.Settings;

public sealed class WebclientConnectionSection
{
    public const string Name = "WebclientConnections";
    public WebClientConnectionSettings Moex { get; set; } = null!;
}