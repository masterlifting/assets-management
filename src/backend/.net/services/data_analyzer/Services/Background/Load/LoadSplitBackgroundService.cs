using AM.Services.Market.Services.Tasks;

namespace AM.Services.Market.Services.Background.Load;

public sealed class LoadSplitBackgroundService : BaseLoadBackgroundService
{
    public LoadSplitBackgroundService(IServiceScopeFactory scopeFactory, ILogger<LoadSplitBackgroundService> logger)
        : base(scopeFactory, logger, new LoadSplitTask(scopeFactory))
    {
    }
}