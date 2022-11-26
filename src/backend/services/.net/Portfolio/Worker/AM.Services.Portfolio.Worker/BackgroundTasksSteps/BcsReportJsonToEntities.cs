using AM.Services.Portfolio.Core.Domain.EntityStateModels.Report.Bcs;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Services.BcsServices;

using Shared.Background.Interfaces;
using Shared.Persistense.Abstractions.Repositories;

using System.Text.Json;

using static Shared.Persistense.Abstractions.Constants.Enums;

namespace AM.Services.Portfolio.Worker.BackgroundTasksSteps;

public class BcsReportJsonToEntities : IProcessableStepHandler<DataAsJson>
{
    private readonly SemaphoreSlim _semaphore = new(1);
    private readonly IRepository _repository;
    public BcsReportJsonToEntities(IRepository repository) => _repository = repository;
    
    public Task HandleStepAsync(IEnumerable<DataAsJson> entities, CancellationToken cToken) =>
        Task.WhenAll(entities.Select(x => Task.Run(async () =>
        {
            try
            {
                var reportModel = x.Json.Deserialize<BcsReportModel>();

                if (reportModel!.Version != x.JsonVersion)
                    throw new Exception();

                var report = new BcsReportJsonSerializer(reportModel, x.UserId, _repository);

                var deals = report.GetDeals();
                var events = report.GetEvents();

                await _semaphore.WaitAsync(cToken);

                await _repository.CreateRangeAsync(deals, cToken);
                await _repository.CreateRangeAsync(events, cToken);

                _semaphore.Release();
            }
            catch (Exception exception)
            {
                x.ProcessStatusId = (int)ProcessableEntityStatuses.Error;
                x.Info = exception.Message;
            }
        }, cToken)));

    public Task<IReadOnlyCollection<DataAsJson>> HandleStepAsync(CancellationToken cToken)
    {
        throw new NotImplementedException();
    }
}
