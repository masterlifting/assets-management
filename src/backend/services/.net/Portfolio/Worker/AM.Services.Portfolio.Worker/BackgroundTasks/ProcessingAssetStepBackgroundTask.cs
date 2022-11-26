using AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Worker.BackgroundTasksSteps;

using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Handlers;
using Shared.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Worker.BackgroundTasks;

public sealed class ProcessingAssetStepBackgroundTask : ProcessingBackgroundTask<Asset, ProcessStep>
{
    public ProcessingAssetStepBackgroundTask(ILogger logger, IRepository repository) 
        : base(logger, repository, new BackgroundTaskStepHandler<Asset>(new() { }))
    {
    }
}