using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;

using Microsoft.Extensions.Options;
using Shared.Background.Core.BackgroundServices;
using Shared.Background.Settings.Sections;

namespace AM.Services.Portfolio.Worker.BackgroundServices;

public sealed class DataAsBytesBackgroundService : EntitiesProcessingBackgroundService<DataAsBytes>
{
    public DataAsBytesBackgroundService(
        IServiceScopeFactory scopeFactory, 
        IOptionsMonitor<BackgroundTaskSection> options, 
        ILogger<DataAsBytesBackgroundService> logger)
         : base(options, logger, scopeFactory) { }
}