using AM.Services.Market.Services.Tasks;

namespace AM.Services.Market.Services.Background.Load;

public sealed class LoadPriceBackgroundService : BaseLoadBackgroundService
{
    public LoadPriceBackgroundService(IServiceScopeFactory scopeFactory, ILogger<LoadPriceBackgroundService> logger)
        : base(scopeFactory, logger, new LoadPriceTask(scopeFactory))
    {
    }
}