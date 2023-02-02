﻿using AM.Services.Portfolio.Core.Abstractions.Persistence;
using AM.Services.Portfolio.Core.Domain.Persistence.Collections;
using AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;
using AM.Services.Portfolio.Infrastructure.Exceptions;
using AM.Services.Portfolio.Infrastructure.Persistence;

using Shared.Background.Interfaces;
using Shared.Queue.Domain.WorkQueue;

using static Shared.Persistence.Abstractions.Constants.Enums;

namespace AM.Services.Portfolio.Worker.BackgroundTasksSteps;

public class BcsReportParser : IProcessStepHandler<IncomingData>
{
    private readonly IUnitOfWorkRepository _uow;
    private readonly IWorkQueue _workQueue;
    private readonly IBcsReportService _service;
    public BcsReportParser(IBcsReportService service, IUnitOfWorkRepository uow, IWorkQueue workQueue)
    {
        _service = service;
        _uow = uow;
        _workQueue = workQueue;
    }
    public Task HandleStepAsync(IEnumerable<IncomingData> entities, CancellationToken cToken) =>
        Task.WhenAll(entities.Select(x => Task.Run(async () =>
        {
            try
            {
                var reportModel = _service.GetReportModel(x.PayloadSource, x.Payload);

                var deals = await _service.GetDealsAsync(x.UserId, reportModel.Agreement, reportModel.Deals);
                var events = await _service.GetEventsAsync(x.UserId, reportModel.Agreement, reportModel.Events);

                var saveChanges = async () =>
                {
                    try
                    {
                        await _uow.PostgreContext.SetTransactionAsync(cToken);
                        await _uow.Deal.Writer.CreateRangeAsync(deals, cToken);
                        await _uow.Event.Writer.CreateRangeAsync(events, cToken);
                        await _uow.PostgreContext.CommitTransactionAsync(cToken);
                    }
                    catch (Exception exeption)
                    {
                        await _uow.PostgreContext.RollbackTransactionAsync(cToken);
                        throw new PortfolioInfrastructureException(nameof(UnitOfWorkRepository), "Save to database form the " + nameof(HandleStepAsync), new(exeption));
                    }
                };

                await _workQueue.ProcessAsync(saveChanges);
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
