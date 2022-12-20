using AM.Services.Portfolio.Core.Abstractions.Persistence;
using AM.Services.Portfolio.Core.Domain.Persistence.Collections;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;
using AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;
using AM.Services.Portfolio.Worker.BackgroundTasksSteps;

using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Handlers;

namespace AM.Services.Portfolio.Worker.BackgroundTasks;

public sealed class BackgroundTaskIncomingDataProcessing : BackgroundTaskProcessing<IncomingData, ProcessStep>
{
    public BackgroundTaskIncomingDataProcessing(ILogger<BackgroundTaskIncomingDataProcessing> logger, IBcsReportService service, IUnitOfWorkRepository uow)
        : base(logger, uow.IncomingData, uow.ProcessStep, new BackgroundTaskStepHandler<IncomingData>(new()
        {
            {(int)Core.Constants.Enums.ProcessSteps.ParseBcsReport, new BcsReportParser(service, uow)}
        }))
    { }
}