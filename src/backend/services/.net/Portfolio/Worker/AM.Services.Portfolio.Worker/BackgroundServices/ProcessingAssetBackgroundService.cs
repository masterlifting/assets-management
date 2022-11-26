using AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Microsoft.Extensions.Options;

using Shared.Background.Core.BackgroundServices;
using Shared.Background.Settings.Sections;

namespace AM.Services.Portfolio.Worker.BackgroundServices;

public sealed class ProcessingAssetBackgroundService : ProcessingBackgroundService<Asset, ProcessStep>
{
    public ProcessingAssetBackgroundService(
        IServiceScopeFactory scopeFactory, 
        IOptionsMonitor<BackgroundTaskSection> options, 
        ILogger<ProcessingAssetBackgroundService> logger)
        : base(options, logger, scopeFactory) { }
}