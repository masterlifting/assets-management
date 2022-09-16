using AM.Services.Portfolio.Host.Services.Background.Base;
using AM.Services.Portfolio.Host.Services.Background.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AM.Services.Portfolio.Host.Services.Background;

public class ReportBackgroundService : EntityStateBackgroundService
{
    public ReportBackgroundService(
        IServiceScopeFactory scopeFactory
        , IOptions<ServiceTaskSection> options
        , ILogger<ReportBackgroundService> logger)
        : base(options, logger, new ReportBackgroundTask("SrvCreateDocBatchCftbOffline", scopeFactory))
    {
    }
}