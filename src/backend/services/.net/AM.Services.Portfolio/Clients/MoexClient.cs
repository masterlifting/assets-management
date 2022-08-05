using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AM.Services.Portfolio.Models.Clients;
using AM.Services.Portfolio.Settings;
using Microsoft.Extensions.Options;
using static AM.Services.Common.Contracts.Enums;

namespace AM.Services.Portfolio.Clients;

public class MoexClient
{
    private readonly HttpClient httpClient;
    private readonly string baseUri;


    public MoexClient(HttpClient httpClient, IOptions<ServiceSettings> options)
    {
        this.httpClient = httpClient;
        var moexSetting = options.Value.ClientSettings.Moex;
        baseUri = $"https://{moexSetting.Host}/iss/engines/stock/markets";
    }

    public async Task<MoexIsinData> GetIsinAsync(string ticker, Countries country)
    {
        var urlIsinPath = GetExchangeUrlPath(country);
        ticker = GetTicker(ticker, country);

        var response = await httpClient.GetFromJsonAsync<MoexIsinData>($"{baseUri}/{urlIsinPath}/securities/{ticker}.json");

        return response ?? throw new HttpRequestException("Data not found from "+ nameof(GetIsinAsync));
    }
    public async Task<MoexIsinData> GetIsinsAsync(Countries country)
    {
        var urlIsinPath = GetExchangeUrlPath(country);

        var response = await httpClient.GetFromJsonAsync<MoexIsinData>($"{baseUri}/{urlIsinPath}/securities.json");

        return response ?? throw new HttpRequestException("Data not found from " + nameof(GetIsinsAsync));
    }

    private static string GetExchangeUrlPath(Countries country) => country switch
    {
        Countries.RUS => "shares/boards/tqbr",
        Countries.CHN or Countries.USA => "foreignshares/boards/tqbd",
        _ => throw new ArgumentOutOfRangeException(nameof(country), country, nameof(GetExchangeUrlPath))
    };

    private static string GetTicker(string ticker, Countries country) =>
        country switch
        {
            Countries.RUS => ticker,
            Countries.USA => $"{ticker}-rm",
            Countries.CHN => $"{ticker}-rm",
            Countries.GBR => ticker,
            Countries.DEU => ticker,
            _ => ticker
        };
}