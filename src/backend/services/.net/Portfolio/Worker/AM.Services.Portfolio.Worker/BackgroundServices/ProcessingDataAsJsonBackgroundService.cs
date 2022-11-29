using AM.Services.Portfolio.Core.Domain.Persistense.Collections;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using Microsoft.Extensions.Options;
using Shared.Background.Core.BackgroundServices;
using Shared.Background.Settings.Sections;

namespace AM.Services.Portfolio.Worker.BackgroundServices;

public sealed class ProcessingDataAsJsonBackgroundService : ProcessingBackgroundService<DataAsJson, ProcessStep>
{
    public ProcessingDataAsJsonBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptionsMonitor<BackgroundTaskSection> options,
        ILogger<ProcessingDataAsJsonBackgroundService> logger)
         : base(options, logger, scopeFactory) { }
}