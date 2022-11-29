using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Handlers;
using Shared.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Worker.BackgroundTasks;

public sealed class ProcessingEventStepBackgroundTask : ProcessingBackgroundTask<Event, ProcessStep>
{
    public ProcessingEventStepBackgroundTask(ILogger<ProcessingEventStepBackgroundTask> logger, IRepository repository)
        : base(logger, repository, new BackgroundTaskStepHandler<Event>(new() { }))
    {
    }
}