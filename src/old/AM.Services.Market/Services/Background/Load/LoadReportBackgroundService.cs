using AM.Services.Market.Services.Tasks;

namespace AM.Services.Market.Services.Background.Load;

public sealed class LoadReportBackgroundService : BaseLoadBackgroundService
{
    public LoadReportBackgroundService(IServiceScopeFactory scopeFactory, ILogger<LoadReportBackgroundService> logger)
        : base(scopeFactory, logger, new LoadReportTask(scopeFactory))
    {
    }
}