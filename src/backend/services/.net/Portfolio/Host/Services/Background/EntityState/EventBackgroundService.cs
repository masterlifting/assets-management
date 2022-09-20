using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Host.Services.Background.EntityState.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Background.Abstractions.Services;
using Shared.Background.Settings.Sections;

namespace AM.Services.Portfolio.Host.Services.Background.EntityState;

public sealed class EventBackgroundService : EntityStateBackgroundService
{
    public EventBackgroundService(
        IServiceScopeFactory scopeFactory
        , IOptionsMonitor<BackgroundTaskSection> options
        , ILogger<EventBackgroundService> logger)
        : base(options, logger, new EventBackgroundTask($"{nameof(Event)}_handling", scopeFactory))
    {
    }
}