using AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;

using Microsoft.Extensions.Options;
using Shared.Background.Core.BackgroundServices;
using Shared.Background.Settings.Sections;

namespace AM.Services.Portfolio.Worker.BackgroundServices;

public sealed class EntitiesProcessingDataAsJsonBackgroundService : EntitiesProcessingBackgroundService<DataAsJson, ProcessStep>
{
    public EntitiesProcessingDataAsJsonBackgroundService(
        IServiceScopeFactory scopeFactory, 
        IOptionsMonitor<BackgroundTaskSection> options, 
        ILogger<EntitiesProcessingDataAsJsonBackgroundService> logger)
         : base(options, logger, scopeFactory) { }
}