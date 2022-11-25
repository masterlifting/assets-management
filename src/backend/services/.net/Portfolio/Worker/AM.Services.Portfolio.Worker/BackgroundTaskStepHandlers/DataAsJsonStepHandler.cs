using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;
using AM.Services.Portfolio.Worker.BackgroundTaskStepHandlers.Steps;

using Shared.Background.Core.Handlers;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums.ProcessSteps;

namespace AM.Services.Portfolio.Worker.BackgroundTaskStepHandlers;

public sealed class DataAsJsonStepHandler : BackgroundTaskStepHandler<DataAsJson>
{
    public DataAsJsonStepHandler() : base(new()
    {
          {(int)ParseBcsJsonModelToEntities, new BcsReportJsonModelToEntitiesParser()}
    })
    {
    }
}