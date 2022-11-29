﻿using AM.Services.Portfolio.Core.Domain.Persistense.Collections;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;
using AM.Services.Portfolio.Worker.BackgroundTasksSteps;

using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Handlers;
using Shared.Persistense.Abstractions.Repositories;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Worker.BackgroundTasks;

public sealed class ProcessingDataAsBytesBackgroundTask : ProcessingBackgroundTask<IncomingData, ProcessStep>
{
    public ProcessingDataAsBytesBackgroundTask(
        ILogger<ProcessingDataAsBytesBackgroundTask> logger
        , IBcsReportDataToJsonService service
        , IPostgreSQLRepository repository)
        : base(logger, repository, new BackgroundTaskStepHandler<IncomingData>(new()
        {
        {(int)ProcessSteps.ParseBcsReportDataToJson, new BcsReportDataToJson(service, repository)}
        }))
    { }
}