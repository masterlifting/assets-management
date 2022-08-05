using AM.Services.Market.Services.Tasks;

namespace AM.Services.Market.Services.Background.Load;

public sealed class LoadFloatBackgroundService : BaseLoadBackgroundService
{
    public LoadFloatBackgroundService(IServiceScopeFactory scopeFactory, ILogger<LoadFloatBackgroundService> logger)
        : base(scopeFactory, logger, new LoadFloatTask(scopeFactory))
    {
    }
}