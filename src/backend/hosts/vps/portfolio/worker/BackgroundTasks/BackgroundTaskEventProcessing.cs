using AM.Services.Portfolio.Core.Abstractions.Persistence;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Handlers;

namespace AM.Services.Portfolio.Worker.BackgroundTasks;

public sealed class BackgroundTaskEventProcessing : BackgroundTaskProcessing<Event, ProcessStep>
{
    public BackgroundTaskEventProcessing(ILogger<BackgroundTaskEventProcessing> logger, IUnitOfWorkRepository unitOfWork)
        : base(logger, unitOfWork.Event, unitOfWork.ProcessStep, new BackgroundTaskStepHandler<Event>(new() { }))
    {
    }
}