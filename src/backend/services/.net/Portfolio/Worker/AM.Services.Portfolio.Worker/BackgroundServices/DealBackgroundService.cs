using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;

using Microsoft.Extensions.Options;
using Shared.Background.Core.BackgroundServices;
using Shared.Background.Settings.Sections;
namespace AM.Services.Portfolio.Worker.BackgroundServices;

public sealed class DealBackgroundService : EntitiesProcessingBackgroundService<Deal>
{
    public DealBackgroundService(
        IServiceScopeFactory scopeFactory, 
        IOptionsMonitor<BackgroundTaskSection> options, 
        ILogger<DealBackgroundService> logger)
         : base(options, logger, scopeFactory) { }
}