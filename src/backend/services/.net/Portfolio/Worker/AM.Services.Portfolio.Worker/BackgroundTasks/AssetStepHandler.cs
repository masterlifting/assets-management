using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;

using Shared.Background.Core.Handlers;

namespace AM.Services.Portfolio.Worker.BackgroundTaskStepHandlers;

public sealed class AssetStepHandler : BackgroundTaskStepHandler<Asset>
{
    public AssetStepHandler() : base(new()) { }
}