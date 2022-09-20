using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Host.Services.Background.EntityState.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Background.Abstractions.Services;
using Shared.Background.Settings.Sections;

namespace AM.Services.Portfolio.Host.Services.Background.EntityState;

public sealed class ReportFileBackgroundService : EntityStateBackgroundService
{
    public ReportFileBackgroundService(
        IServiceScopeFactory scopeFactory
        , IOptionsMonitor<BackgroundTaskSection> options
        , ILogger<ReportFileBackgroundService> logger)
        : base(options, logger, new ReportFileBackgroundTask($"{nameof(ReportFile)}_handling", scopeFactory))
    {
    }
}