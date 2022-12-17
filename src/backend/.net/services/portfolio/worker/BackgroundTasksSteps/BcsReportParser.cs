using AM.Services.Portfolio.Core.Abstractions.Persistence;
using AM.Services.Portfolio.Core.Domain.Persistence.Collections;
using AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;

using Shared.Background.Interfaces;
using Shared.Persistence.Abstractions.Repositories;

using static Shared.Persistence.Abstractions.Constants.Enums;

namespace AM.Services.Portfolio.Worker.BackgroundTasksSteps;

public class BcsReportParser : IProcessStepHandler<IncomingData>
{
    private readonly SemaphoreSlim _semaphore = new(1);

    private readonly IUnitOfWorkRepository _uow;
    private readonly IBcsReportService _service;

    public BcsReportParser(IBcsReportService service, IUnitOfWorkRepository uow)
    {
        _service = service;
        _uow = uow;
    }

    public Task HandleStepAsync(IEnumerable<IncomingData> entities, CancellationToken cToken) =>
        Task.WhenAll(entities.Select(x => Task.Run(async () =>
        {
            try
            {
                var reportModel = _service.GetReportModel(x.PayloadSource, x.Payload);

                var deals = _service.GetDeals(reportModel);
                var events = _service.GetEvents(reportModel);

                await _semaphore.WaitAsync(cToken);

                await _uow.Deal.Writer.CreateRangeAsync(deals, cToken);
                await _uow.Event.Writer.CreateRangeAsync(events, cToken);
            }
            catch (Exception exception)
            {
                x.ProcessStatusId = (int)ProcessStatuses.Error;
                x.Error = $"Exception in the file: {x.PayloadSource}. Error: " + exception.Message;
            }
            finally
            {
                _semaphore.Release();
            }
        }, cToken)));

    public Task<IReadOnlyCollection<IncomingData>> HandleStepAsync(CancellationToken cToken)
    {
        throw new NotImplementedException();
    }
}