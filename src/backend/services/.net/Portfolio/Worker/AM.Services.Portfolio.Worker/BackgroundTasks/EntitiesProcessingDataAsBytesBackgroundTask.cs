using AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;
using AM.Services.Portfolio.Worker.BackgroundTaskStepHandlers.Steps;

using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Handlers;
using Shared.Persistense.Abstractions.Repositories;

namespace AM.Services.Portfolio.Worker.BackgroundTaskStepHandlers;

public sealed class EntitiesProcessingDataAsBytesBackgroundTask : EntitiesProcessingBackgroundTask<DataAsBytes, ProcessStep>
{
    public EntitiesProcessingDataAsBytesBackgroundTask(
        ILogger logger
        , IMongoDBRepository repository) : base(logger, repository, new BackgroundTaskStepHandler<DataAsBytes>(new ()
        {
            {BcsReportDataToJsonModelParser.StepId, new BcsReportDataToJsonModelParser()}
        })) { }
}