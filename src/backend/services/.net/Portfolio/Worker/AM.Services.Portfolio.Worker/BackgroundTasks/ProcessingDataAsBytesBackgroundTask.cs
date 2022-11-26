using AM.Services.Portfolio.Core.Abstractions.Excel;
using AM.Services.Portfolio.Core.Domain.Persistense.Catalogs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Worker.BackgroundTasksSteps;

using Shared.Background.Core.BackgroundTasks;
using Shared.Background.Core.Handlers;
using Shared.Persistense.Abstractions.Repositories;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Worker.BackgroundTasks;

public sealed class ProcessingDataAsBytesBackgroundTask : ProcessingBackgroundTask<DataAsBytes, ProcessStep>
{
    public ProcessingDataAsBytesBackgroundTask(ILogger logger, IExcelService excelService, IPostgreSQLRepository repository) 
        : base(logger, repository, new BackgroundTaskStepHandler<DataAsBytes>(new()
        {
            {(int)ProcessSteps.ParseBcsReportDataToJson, new BcsReportDataToJson(excelService, repository)}
        }))
    { }
}