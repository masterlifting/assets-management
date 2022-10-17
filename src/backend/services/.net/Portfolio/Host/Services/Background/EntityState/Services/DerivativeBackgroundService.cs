using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Host.Services.Background.EntityState.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Shared.Background.Abstractions.Services;
using Shared.Background.Settings.Sections;

namespace AM.Services.Portfolio.Host.Services.Background.EntityState.Services;

public sealed class DerivativeBackgroundService : EntityStateBackgroundService
{
    public DerivativeBackgroundService(
        IServiceScopeFactory scopeFactory
        , IOptionsMonitor<BackgroundTaskSection> options
        , ILogger<DerivativeBackgroundService> logger)
        : base(options, logger, new DerivativeBackgroundTask($"{nameof(Derivative)}_handling", scopeFactory))
    {
    }
}