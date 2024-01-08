using AM.Services.Portfolio.Core.Abstractions.Persistence;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Handlers;

namespace AM.Services.Portfolio.Worker.BackgroundTasks;

public sealed class BackgroundTaskAssetProcessing : BackgroundTaskProcessing<Asset, ProcessStep>
{
    public BackgroundTaskAssetProcessing(ILogger<BackgroundTaskAssetProcessing> logger, IUnitOfWorkRepository unitOfWork)
        : base(logger, unitOfWork.Asset, unitOfWork.ProcessStep, new BackgroundTaskStepHandler<Asset>(new() { }))
    {
    }
}