using AM.Services.Common.Contracts.Entities.Enums;
using AM.Services.Portfolio.Core.Models.Clients;

namespace AM.Services.Portfolio.Core.Interfaces.External.Webclients;

public interface IMoexWebclient
{
    Task<MoexIsinData> GetIsinAsync(string ticker, Countries country);
    Task<MoexIsinData> GetIsinsAsync(Countries country);
}