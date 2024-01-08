using Net.Shared.Web.Models.Settings.Connections;

namespace AM.Portfolio.Infrastructure.Settings;

public sealed class WebclientConnectionSection
{
    public const string Name = "WebClientConnections";
    public HttpConnectionSettings Moex { get; set; } = null!;
}
