using AM.Services.Market.Services.Tasks;

namespace AM.Services.Market.Services.Background.Load;

public sealed class LoadDividendBackgroundService : BaseLoadBackgroundService
{
    public LoadDividendBackgroundService(IServiceScopeFactory scopeFactory, ILogger<LoadDividendBackgroundService> logger)
        : base(scopeFactory, logger, new LoadDividendTask(scopeFactory))
    {
    }
}