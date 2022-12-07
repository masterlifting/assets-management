using AM.Services.Portfolio.Core.Abstractions.WebServices;
using AM.Services.Portfolio.Core.Models.WebClient;
using AM.Services.Portfolio.Infrastructure.Exceptions;
using AM.Services.Portfolio.Infrastructure.Settings;

using Microsoft.Extensions.Options;

using System.Net.Http.Json;

using static AM.Services.Common.Constants.Enums;

namespace AM.Services.Portfolio.Infrastructure.WebClients;

public sealed class MoexWebclient : IMoexWebclient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUri;


    public MoexWebclient(IHttpClientFactory httpClientFactory, IOptions<WebclientConnectionSection> options)
    {
        _httpClient = httpClientFactory.CreateClient();
        var moexSetting = options.Value.Moex;
        _baseUri = $"https://{moexSetting.Host}/iss/engines/stock/markets";
    }

    public async Task<MoexIsinData> GetIsinAsync(string ticker, Countries country)
    {
        var urlIsinPath = GetExchangeUrlPath(country);
        ticker = GetTicker(ticker, country);

        var response = await _httpClient.GetFromJsonAsync<MoexIsinData>($"{_baseUri}/{urlIsinPath}/securities/{ticker}.json");

        return response ?? throw new PortfolioInfrastructureException(nameof(MoexWebclient), nameof(GetIsinsAsync), new("No response"));
    }
    public async Task<MoexIsinData> GetIsinsAsync(Countries country)
    {
        var urlIsinPath = GetExchangeUrlPath(country);

        var response = await _httpClient.GetFromJsonAsync<MoexIsinData>($"{_baseUri}/{urlIsinPath}/securities.json");

        return response ?? throw new PortfolioInfrastructureException(nameof(MoexWebclient), nameof(GetIsinsAsync), new("No response"));
    }

    private static string GetExchangeUrlPath(Countries country) => country switch
    {
        Countries.Rus => "shares/boards/tqbr",
        Countries.Chn or Countries.Usa => "foreignshares/boards/tqbd",
        _ => throw new ArgumentOutOfRangeException(nameof(country), country, nameof(GetExchangeUrlPath))
    };
    private static string GetTicker(string ticker, Countries country) =>
        country switch
        {
            Countries.Rus => ticker,
            Countries.Usa => $"{ticker}-rm",
            Countries.Chn => $"{ticker}-rm",
            Countries.Gbr => ticker,
            Countries.Deu => ticker,
            _ => ticker
        };
}