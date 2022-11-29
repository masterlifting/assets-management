using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Handlers;
using Shared.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Worker.BackgroundTasks;

public sealed class ProcessingDerivativeStepBackgroundTask : ProcessingBackgroundTask<Derivative, ProcessStep>
{
    public ProcessingDerivativeStepBackgroundTask(ILogger<ProcessingDerivativeStepBackgroundTask> logger, IRepository repository)
        : base(logger, repository, new BackgroundTaskStepHandler<Derivative>(new() { }))
    {
    }
}