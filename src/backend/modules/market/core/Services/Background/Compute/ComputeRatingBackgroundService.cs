using AM.Services.Market.Services.Tasks;

namespace AM.Services.Market.Services.Background.Compute;

public sealed class ComputeRatingBackgroundService : BaseComputeBackgroundService
{
    public ComputeRatingBackgroundService(IServiceScopeFactory scopeFactory, ILogger<ComputeRatingBackgroundService> logger)
        : base(scopeFactory, logger, new ComputeRatingTask(scopeFactory))
    {
    }
}