using AM.Services.Common.Contracts.Models.Configuration;

namespace AM.Services.Portfolio.Settings.Sections;

public class ClientSettings
{
    public HostModel Moex { get; set; } = null!;
}