using AM.Services.Common.Contracts.Models.Configuration;

namespace AM.Services.Market.Models.Settings;

public class InvestingModel : HostModel
{
    public string Path { get; set; } = null!;
    public string Financial { get; set; } = null!;
    public string Balance { get; set; } = null!;
    public string Dividends { get; set; } = null!;
}