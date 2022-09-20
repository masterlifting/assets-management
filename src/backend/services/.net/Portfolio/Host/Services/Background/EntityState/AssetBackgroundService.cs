using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Host.Services.Background.EntityState.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Background.Abstractions.Services;
using Shared.Background.Settings.Sections;

namespace AM.Services.Portfolio.Host.Services.Background.EntityState;

public sealed class AssetBackgroundService : EntityStateBackgroundService
{
    public AssetBackgroundService(
        IServiceScopeFactory scopeFactory
        , IOptionsMonitor<BackgroundTaskSection> options
        , ILogger<AssetBackgroundService> logger)
        : base(options, logger, new AssetBackgroundTask($"{nameof(Asset)}_handling", scopeFactory))
    {
    }
}