using AM.Services.Common.Contracts.Models.Configuration;
using AM.Services.Market.Models.Settings;

namespace AM.Services.Market.Settings.Sections;

public class ClientSettings
{
    public HostModel Moex { get; set; } = null!;
    public HostModel TdAmeritrade { get; set; } = null!;
    public InvestingModel Investing { get; set; } = null!;
}