using AM.Services.Common.Contracts.Models.Configuration;
using AM.Services.Portfolio.Settings.Sections;

namespace AM.Services.Portfolio.Settings;

public class ServiceSettings
{
    public ClientSettings ClientSettings { get; set; } = null!;
    public ConnectionStrings ConnectionStrings { get; set; } = null!;
}