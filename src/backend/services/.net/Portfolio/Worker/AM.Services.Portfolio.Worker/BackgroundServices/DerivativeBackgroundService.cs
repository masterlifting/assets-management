using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;

using Microsoft.Extensions.Options;
using Shared.Background.Core.BackgroundServices;
using Shared.Background.Settings.Sections;
namespace AM.Services.Portfolio.Worker.BackgroundServices;

public sealed class DerivativeBackgroundService : EntitiesProcessingBackgroundService<Derivative>
{
    public DerivativeBackgroundService(
        IServiceScopeFactory scopeFactory, 
        IOptionsMonitor<BackgroundTaskSection> options, 
        ILogger<DerivativeBackgroundService> logger)
        : base(options, logger, scopeFactory) { }
}