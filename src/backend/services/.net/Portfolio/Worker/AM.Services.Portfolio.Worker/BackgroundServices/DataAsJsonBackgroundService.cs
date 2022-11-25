using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;

using Microsoft.Extensions.Options;
using Shared.Background.Core.BackgroundServices;
using Shared.Background.Settings.Sections;

namespace AM.Services.Portfolio.Worker.BackgroundServices;

public sealed class DataAsJsonBackgroundService : EntitiesProcessingBackgroundService<DataAsJson>
{
    public DataAsJsonBackgroundService(
        IServiceScopeFactory scopeFactory, 
        IOptionsMonitor<BackgroundTaskSection> options, 
        ILogger<DataAsJsonBackgroundService> logger)
         : base(options, logger, scopeFactory) { }
}