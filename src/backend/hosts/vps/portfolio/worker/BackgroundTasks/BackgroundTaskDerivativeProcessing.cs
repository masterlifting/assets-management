using AM.Services.Portfolio.Core.Abstractions.Persistence;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Handlers;

namespace AM.Services.Portfolio.Worker.BackgroundTasks;

public sealed class BackgroundTaskDerivativeProcessing : BackgroundTaskProcessing<Derivative, ProcessStep>
{
    public BackgroundTaskDerivativeProcessing(ILogger<BackgroundTaskDerivativeProcessing> logger, IUnitOfWorkRepository unitOfWork)
        : base(logger, unitOfWork.Derivative, unitOfWork.ProcessStep, new BackgroundTaskStepHandler<Derivative>(new() { }))
    {
    }
}