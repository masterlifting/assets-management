using AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;

using Microsoft.Extensions.Options;
using Shared.Background.Core.BackgroundServices;
using Shared.Background.Settings.Sections;

namespace AM.Services.Portfolio.Worker.BackgroundServices;

public sealed class EntitiesProcessingDataAsBytesBackgroundService : EntitiesProcessingBackgroundService<DataAsBytes, ProcessStep>
{
    public EntitiesProcessingDataAsBytesBackgroundService(
        IServiceScopeFactory scopeFactory, 
        IOptionsMonitor<BackgroundTaskSection> options, 
        ILogger<EntitiesProcessingDataAsBytesBackgroundService> logger)
         : base(options, logger, scopeFactory) { }
}