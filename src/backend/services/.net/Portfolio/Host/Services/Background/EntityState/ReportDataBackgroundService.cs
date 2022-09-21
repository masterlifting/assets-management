using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Host.Services.Background.EntityState.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Shared.Background.Abstractions.Services;
using Shared.Background.Settings.Sections;

namespace AM.Services.Portfolio.Host.Services.Background.EntityState
{
    public sealed class ReportDataBackgroundService : EntityStateBackgroundService
    {
        public ReportDataBackgroundService(
            IServiceScopeFactory scopeFactory
            , IOptionsMonitor<BackgroundTaskSection> options
            , ILogger<ReportDataBackgroundService> logger)
            : base(options, logger, new ReportFileBackgroundTask($"{nameof(ReportData)}_handling", scopeFactory))
        {
        }
    }
}