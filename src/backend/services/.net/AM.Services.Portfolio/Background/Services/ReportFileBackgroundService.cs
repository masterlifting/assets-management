using AM.Services.Portfolio.Background.ServiceTasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Contracts.Settings.Sections;
using Shared.Core.Background.EntityState;

namespace AM.Services.Portfolio.Background.Services;

public class ReportFileBackgroundService : EntityStateBackgroundService
{
    protected ReportFileBackgroundService(
        IServiceScopeFactory scopeFactory
        , IOptionsMonitor<BackgroundTaskSection> options
        , ILogger<ReportFileBackgroundService> logger)
        : base(options, logger, new ReportFileBackgroundTask(scopeFactory))
    {
    }
}