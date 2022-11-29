using AM.Services.Portfolio.Core.Models.WebClient;

using static AM.Services.Common.Contracts.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Abstractions.WebServices;

public interface IMoexWebclient
{
    Task<MoexIsinData> GetIsinAsync(string ticker, Countries country);
    Task<MoexIsinData> GetIsinsAsync(Countries country);
}