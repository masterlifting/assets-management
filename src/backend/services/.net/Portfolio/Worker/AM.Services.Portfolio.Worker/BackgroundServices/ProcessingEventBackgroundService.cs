using AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Microsoft.Extensions.Options;
using Shared.Background.Core.BackgroundServices;
using Shared.Background.Settings.Sections;
namespace AM.Services.Portfolio.Worker.BackgroundServices;

public sealed class ProcessingEventBackgroundService : ProcessingBackgroundService<Event, ProcessStep>
{
    public ProcessingEventBackgroundService(
        IServiceScopeFactory scopeFactory, 
        IOptionsMonitor<BackgroundTaskSection> options, 
        ILogger<ProcessingEventBackgroundService> logger)
         : base(options, logger, scopeFactory) { }
}