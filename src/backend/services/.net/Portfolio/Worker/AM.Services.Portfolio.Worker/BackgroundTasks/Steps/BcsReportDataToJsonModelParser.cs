using AM.Services.Portfolio.Core.Abstractions.Excel;
using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;
using AM.Services.Portfolio.Core.Services.BcsServices;

using Shared.Background.Interfaces;
using Shared.Extensions.Serialization;

using System.Text.Json;

using static AM.Services.Portfolio.Core.Constants;
using static Shared.Persistense.Abstractions.Constants.Enums;
using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Worker.BackgroundTaskStepHandlers.Steps;

public class BcsReportDataToJsonModelParser : IProcessableStepHandler<DataAsBytes>
{
    public const int StepId = (int)ProcessSteps.ParseBcsReportToJson;

    private readonly SemaphoreSlim _semaphore = new(1);
    private readonly IExcelService _excelService;
    private readonly IDataAsJsonRepository _processedModelRepository;

    public BcsReportDataToJsonModelParser(
        IExcelService excelService
        , IDataAsJsonRepository processedModelRepository)
    {
        _excelService = excelService;
        _processedModelRepository = processedModelRepository;
    }
    public Task HandleStepAsync(IEnumerable<DataAsBytes> entities, CancellationToken cToken) =>
        Task.WhenAll(entities.Select(x => Task.Run(async () =>
        {
            try
            {
                var parser = new BcsReportDataParser(_excelService, x.Payload);
                var reportModel = parser.GetReportModel();

                var processedModel = new DataAsJson
                {
                    UserId = x.UserId,
                    Json = JsonDocument.Parse(reportModel.Serialize()),
                    JsonVersion = reportModel.Version,
                    ProcessStatusId = (int)ProcessableEntityStatuses.Ready,
                    ProcessStepId = (int)ProcessSteps.ParseBcsJsonModelToEntities
                };

                await _semaphore.WaitAsync(cToken);
                await _processedModelRepository.CreateAsync(processedModel);
                _semaphore.Release();
            }
            catch (Exception exception)
            {
                x.ProcessStatusId = (int)ProcessableEntityStatuses.Error;
                x.Info = $"File: {x.PayloadSource}. " + exception.Message;
            }
        }, cToken)));

    public Task<IReadOnlyCollection<DataAsBytes>> HandleStepAsync(CancellationToken cToken)
    {
        throw new NotImplementedException();
    }
}
