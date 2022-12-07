using AM.Services.Portfolio.Core.Domain.Persistence.Collections;
using AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;
using AM.Services.Portfolio.Worker.BackgroundTasksSteps;

using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Handlers;
using Shared.Persistence.Abstractions.Repositories;

namespace AM.Services.Portfolio.Worker.BackgroundTasks;

public sealed class BackgroundTaskIncomingDataProcessing : BackgroundTaskProcessing<IncomingData, ProcessSteps>
{
    public BackgroundTaskIncomingDataProcessing(
        ILogger<BackgroundTaskIncomingDataProcessing> logger
        , IBcsReportService service
        , IPostgreSQLRepository postgreSQLRepository
        , IMongoDBRepository mongoDBRepository)
        : base(logger, mongoDBRepository, new BackgroundTaskStepHandler<IncomingData>(new()
        {
        {(int)Core.Constants.Enums.ProcessSteps.ParseBcsReport, new BcsReportParser(service, postgreSQLRepository)}
        }))
    { }
}