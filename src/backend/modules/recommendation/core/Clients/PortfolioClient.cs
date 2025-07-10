using System.Net.Http;
using AM.Services.Recommendations.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using static AM.Services.Common.Contracts.Helpers.HttpHelper;

namespace AM.Services.Recommendations.Clients;

public class PortfolioClient : RestClient
{
    public PortfolioClient(IMemoryCache cache, HttpClient httpClient, IOptions<ServiceSettings> options)
        : base(cache, httpClient, options.Value.ClientSettings.Portfolio) { }
}