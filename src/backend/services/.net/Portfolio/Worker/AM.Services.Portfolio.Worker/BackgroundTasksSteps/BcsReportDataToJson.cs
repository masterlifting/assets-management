using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;

using Shared.Background.Interfaces;
using Shared.Extensions.Serialization;
using Shared.Persistense.Abstractions.Repositories;

using System.Text.Json;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;
using static Shared.Persistense.Abstractions.Constants.Enums;

namespace AM.Services.Portfolio.Worker.BackgroundTasksSteps;

public class BcsReportDataToJson : IProcessableStepHandler<DataAsBytes>
{
    private readonly SemaphoreSlim _semaphore = new(1);

    private readonly IPostgreSQLRepository _repository;
    private readonly IBcsReportDataToJsonService _service;

    public BcsReportDataToJson(IBcsReportDataToJsonService service, IPostgreSQLRepository repository)
    {
        _service = service;
        _repository = repository;
    }

    public Task HandleStepAsync(IEnumerable<DataAsBytes> entities, CancellationToken cToken) =>
        Task.WhenAll(entities.Select(x => Task.Run(async () =>
        {
            try
            {
                var reportModel = _service.GetReportModel(x.Payload);

                await _semaphore.WaitAsync(cToken);

                await _repository.CreateAsync(new DataAsJson
                {
                    UserId = x.UserId,
                    Json = JsonDocument.Parse(reportModel.Serialize()),
                    JsonVersion = reportModel.Version,
                    ProcessStepId = (int)ProcessSteps.ParseBcsJsonToEntities,
                    ProcessStatusId = (int)ProcessableEntityStatuses.Ready
                });
            }
            catch (Exception exception)
            {
                x.ProcessStatusId = (int)ProcessableEntityStatuses.Error;
                x.Info = $"File: {x.PayloadSource}. " + exception.Message;
            }
            finally
            {
                _semaphore.Release();
            }
        }, cToken)));

    public Task<IReadOnlyCollection<DataAsBytes>> HandleStepAsync(CancellationToken cToken)
    {
        throw new NotImplementedException();
    }
}
