using AM.Services.Portfolio.Core.Domain.EntityStateModels.Report.Bcs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;

using Shared.Background.Interfaces;
using Shared.Persistense.Abstractions.Repositories;

using System.Text.Json;

using static Shared.Persistense.Abstractions.Constants.Enums;

namespace AM.Services.Portfolio.Worker.BackgroundTasksSteps;

public class BcsReportJsonToEntities : IProcessableStepHandler<DataAsJson>
{
    private readonly SemaphoreSlim _semaphore = new(1);
    private readonly IBcsReportJsonToEntitiesService _service;
    private readonly IPostgreSQLRepository _repository;
    public BcsReportJsonToEntities(IBcsReportJsonToEntitiesService service, IPostgreSQLRepository repository)
    {
        _service = service;
        _repository = repository;
    }

    public Task HandleStepAsync(IEnumerable<DataAsJson> entities, CancellationToken cToken) =>
        Task.WhenAll(entities.Select(x => Task.Run(async () =>
        {
            try
            {
                var reportModel = x.Json.Deserialize<BcsReportModel>();

                if (reportModel!.Version != x.JsonVersion)
                    throw new Exception("The JSON versions aren't equal");

                var deals = _service.GetDeals(reportModel);
                var events = _service.GetEvents(reportModel);

                await _semaphore.WaitAsync(cToken);

                await _repository.CreateRangeAsync(deals, cToken);
                await _repository.CreateRangeAsync(events, cToken);
            }
            catch (Exception exception)
            {
                x.ProcessStatusId = (int)ProcessableEntityStatuses.Error;
                x.Info = exception.Message;
            }
            finally
            {
                _semaphore.Release();
            }
        }, cToken)));

    public Task<IReadOnlyCollection<DataAsJson>> HandleStepAsync(CancellationToken cToken)
    {
        throw new NotImplementedException();
    }
}
