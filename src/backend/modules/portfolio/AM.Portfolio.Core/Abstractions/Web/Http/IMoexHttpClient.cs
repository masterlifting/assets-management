using AM.Portfolio.Core.Models.Web.Http;

using static AM.Shared.Models.Constants.Enums;

namespace AM.Portfolio.Core.Abstractions.Web.Http;

public interface IMoexHttpClient
{
    Task<MoexIsinData> GetIsin(string ticker, Zones zone);
    Task<MoexIsinData> GetIsins(Zones zone);
}
