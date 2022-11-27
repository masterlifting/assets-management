using AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Handlers;
using Shared.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Worker.BackgroundTasks;

public sealed class ProcessingDealStepBackgroundTask : ProcessingBackgroundTask<Deal, ProcessStep>
{
    public ProcessingDealStepBackgroundTask(ILogger<ProcessingDealStepBackgroundTask> logger, IRepository repository)
        : base(logger, repository, new BackgroundTaskStepHandler<Deal>(new() { }))
    {
    }
}