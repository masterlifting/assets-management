using AM.Services.Common.Contracts.Models.Configuration;
using AM.Services.Market.Settings.Sections;

namespace AM.Services.Market.Settings;

public class ServiceSettings
{
    public ClientSettings ClientSettings { get; set; } = null!;
    public ConnectionStrings ConnectionStrings { get; set; } = null!;
    public LoadDataSettings LoadData { get; set; } = null!;
    public ComputeDataSettings ComputeData { get; set; } = null!;
}