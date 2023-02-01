using AM.Services.Portfolio.Core.Abstractions.Persistence;
using AM.Services.Portfolio.Core.Domain.Persistence.Collections;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;

using Shared.Background.Interfaces;

using static Shared.Persistence.Abstractions.Constants.Enums;

namespace AM.Services.Portfolio.Worker.BackgroundTasksSteps;

public class BcsReportParser : IProcessStepHandler<IncomingData>
{
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

                /*/
                var deals = await _service.GetDealsAsync(x.UserId, reportModel.Agreement, reportModel.Deals);
                var events = await _service.GetEventsAsync(x.UserId, reportModel.Agreement, reportModel.Events);
                /*/
                var deals = new List<Deal> { new() };
                var events = new List<Event> { new() };
                //*/

                var dealsCreateTask = _uow.Deal.Writer.CreateRangeAsync(deals, cToken);
                var eventsCreateTask = _uow.Event.Writer.CreateRangeAsync(events, cToken);

                await _uow.ProcessQueueAsync(dealsCreateTask, eventsCreateTask);
            }
            catch (Exception exception)
            {
                x.ProcessStatusId = (int)ProcessStatuses.Error;
                x.Error = $"Source: {x.PayloadSource}. " + exception.Message;
            }
        }, cToken)));
    public Task<IReadOnlyCollection<IncomingData>> HandleStepAsync(CancellationToken cToken)
    {
        throw new NotImplementedException();
    }
}