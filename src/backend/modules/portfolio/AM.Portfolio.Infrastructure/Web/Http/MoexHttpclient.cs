using System.Net.Http.Json;

using AM.Portfolio.Core.Abstractions.Web.Http;
using AM.Portfolio.Core.Models.Web.Http;
using AM.Portfolio.Infrastructure.Exceptions;
using AM.Portfolio.Infrastructure.Settings;

using Microsoft.Extensions.Options;

using static AM.Shared.Models.Constants.Enums;

namespace AM.Portfolio.Infrastructure.Web.Http;

public sealed class MoexHttpclient : IMoexHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUri;

    public MoexHttpclient(IHttpClientFactory httpClientFactory, IOptions<WebclientConnectionSection> options)
    {
        _httpClient = httpClientFactory.CreateClient(nameof(MoexHttpclient));

        var moexSetting = options.Value.Moex;

        _baseUri = $"https://{moexSetting.Host}/iss/engines/stock/markets";
    }

    public async Task<MoexIsinData> GetIsin(string ticker, Zones zone)
    {
        var urlIsinPath = GetExchangeUrlPath(zone);
        ticker = GetTicker(ticker, zone);

        var response = await _httpClient.GetFromJsonAsync<MoexIsinData>($"{_baseUri}/{urlIsinPath}/securities/{ticker}.json");

        return response ?? throw new PortfolioInfrastructureException("Response is null");
    }
    public async Task<MoexIsinData> GetIsins(Zones zone)
    {
        var urlIsinPath = GetExchangeUrlPath(zone);

        var response = await _httpClient.GetFromJsonAsync<MoexIsinData>($"{_baseUri}/{urlIsinPath}/securities.json");

        return response ?? throw new PortfolioInfrastructureException("Response is null");
    }

    private static string GetExchangeUrlPath(Zones zone) => zone switch
    {
        Zones.Rus => "shares/boards/tqbr",
        Zones.Chn or Zones.Usa => "foreignshares/boards/tqbd",
        _ => throw new ArgumentOutOfRangeException(nameof(zone), zone, nameof(GetExchangeUrlPath))
    };
    private static string GetTicker(string ticker, Zones zone) =>
        zone switch
        {
            Zones.Rus => ticker,
            Zones.Usa => $"{ticker}-rm",
            Zones.Chn => $"{ticker}-rm",
            Zones.Gbr => ticker,
            Zones.Deu => ticker,
            _ => ticker
        };
}
