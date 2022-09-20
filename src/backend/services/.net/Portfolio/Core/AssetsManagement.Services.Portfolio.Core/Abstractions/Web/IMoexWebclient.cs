using AM.Services.Portfolio.Core.Models.Clients;

using static AM.Services.Common.Contracts.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Abstractions.Web;

public interface IMoexWebclient
{
    Task<MoexIsinData> GetIsinAsync(string ticker, Countries country);
    Task<MoexIsinData> GetIsinsAsync(Countries country);
}