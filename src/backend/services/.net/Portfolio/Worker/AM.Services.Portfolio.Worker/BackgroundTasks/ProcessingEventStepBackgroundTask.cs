using AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Handlers;
using Shared.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Worker.BackgroundTasks;

public sealed class ProcessingEventStepBackgroundTask : ProcessingBackgroundTask<Event, ProcessStep>
{
    public ProcessingEventStepBackgroundTask(ILogger logger, IRepository repository)
        : base(logger, repository, new BackgroundTaskStepHandler<Event>(new() { }))
    {
    }
}
