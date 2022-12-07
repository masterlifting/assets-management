using AM.Services.Common.Contracts.Models.Configuration;

namespace AM.Services.Recommendations.Settings.Sections;

public class ClientSettings
{
    public HostModel Market { get; set; } = null!;
    public HostModel Portfolio { get; set; } = null!;
}