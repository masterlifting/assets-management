using AM.Services.Common.Contracts.Background;

namespace AM.Services.Market.Services.Tasks;

public sealed class LoadDividendTask : IBackgroundTask
{
    private readonly IServiceScopeFactory scopeFactory;
    public LoadDividendTask(IServiceScopeFactory scopeFactory) => this.scopeFactory = scopeFactory;

    public Task StartAsync<T>(T settings) where T : BackgroundTaskSettings
    {
        throw new NotImplementedException();
    }
}