using AM.Services.Common.Contracts.Abstractions.Persistence.Entities.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;

using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Handlers;
using Shared.Persistence.Abstractions.Repositories;

namespace AM.Services.Portfolio.Worker.BackgroundTasks;

public sealed class BackgroundTaskAssetProcessing : BackgroundTaskProcessing<Asset, ProcessStep>
{
    public BackgroundTaskAssetProcessing(ILogger<BackgroundTaskAssetProcessing> logger, IPersistenceRepository repository)
        : base(logger, repository, new BackgroundTaskStepHandler<Asset>(new() { }))
    {
    }
}