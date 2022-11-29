using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using Microsoft.Extensions.Options;
using Shared.Background.Core.BackgroundServices;
using Shared.Background.Settings.Sections;
namespace AM.Services.Portfolio.Worker.BackgroundServices;

public sealed class ProcessingDerivativeBackgroundService : ProcessingBackgroundService<Derivative, ProcessStep>
{
    public ProcessingDerivativeBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptionsMonitor<BackgroundTaskSection> options,
        ILogger<ProcessingDerivativeBackgroundService> logger)
        : base(options, logger, scopeFactory) { }
}