using AM.Services.Portfolio.Core.Domain.Persistense.Collections;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;
using AM.Services.Portfolio.Worker.BackgroundTasksSteps;

using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Handlers;
using Shared.Persistense.Abstractions.Repositories;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Worker.BackgroundTasks;

public sealed class ProcessingDataAsJsonBackgroundTask : ProcessingBackgroundTask<DataAsJson, ProcessStep>
{
    public ProcessingDataAsJsonBackgroundTask(
        ILogger<ProcessingDataAsJsonBackgroundTask> logger
        , IBcsReportJsonToEntitiesService service
        , IPostgreSQLRepository repository)
        : base(logger, repository, new BackgroundTaskStepHandler<DataAsJson>(new()
        {
        {(int)ProcessSteps.ParseBcsJsonToEntities, new BcsReportJsonToEntities(service, repository)}
        }))
    {
    }
}