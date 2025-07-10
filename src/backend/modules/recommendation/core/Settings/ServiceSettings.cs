using AM.Services.Common.Contracts.Models.Configuration;
using AM.Services.Recommendations.Settings.Sections;

namespace AM.Services.Recommendations.Settings;

public class ServiceSettings
{
    public ClientSettings ClientSettings { get; set; } = null!;
    public ConnectionStrings ConnectionStrings { get; set; } = null!;
    public SaleSettings SaleSettings { get; set; } = null!;
    public PurchaseSettings PurchaseSettings { get; set; } = null!;
}