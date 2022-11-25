using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;

using Shared.Background.Core.Handlers;

namespace AM.Services.Portfolio.Worker.BackgroundTaskStepHandlers;

public sealed class DealStepHandler : BackgroundTaskStepHandler<Deal>
{
    public DealStepHandler() : base(new()) { }
}