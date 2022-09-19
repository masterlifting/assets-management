using System.Net.Http.Json;

using AM.Services.Common.Contracts.Entities.Enums;
using AM.Services.Portfolio.Core.Abstractions.External.Webclients;
using AM.Services.Portfolio.Core.Models.Clients;
using AM.Services.Portfolio.Infrastructure.Settings;

using Microsoft.Extensions.Options;

namespace AM.Services.Portfolio.Infrastructure.External.Webclients;

public class MoexWebclient : IMoexWebclient
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

        return response ?? throw new HttpRequestException("Data not found from "+ nameof(GetIsinAsync));
    }
    public async Task<MoexIsinData> GetIsinsAsync(Countries country)
    {
        var urlIsinPath = GetExchangeUrlPath(country);

        var response = await _httpClient.GetFromJsonAsync<MoexIsinData>($"{_baseUri}/{urlIsinPath}/securities.json");

        return response ?? throw new HttpRequestException("Data not found from " + nameof(GetIsinsAsync));
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