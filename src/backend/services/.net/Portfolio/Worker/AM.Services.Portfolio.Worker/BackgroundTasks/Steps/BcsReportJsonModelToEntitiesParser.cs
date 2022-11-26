using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.EntityStateModels.Report.Bcs;
using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;
using AM.Services.Portfolio.Core.Services.BcsServices;

using Shared.Background.Interfaces;

using System.Text.Json;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;
using static Shared.Persistense.Abstractions.Constants.Enums;

namespace AM.Services.Portfolio.Worker.BackgroundTaskStepHandlers.Steps;

public class BcsReportJsonModelToEntitiesParser : IProcessableStepHandler<DataAsJson>
{
    public const int StepId = (int)ProcessSteps.ParseBcsReportToJson;

    private readonly SemaphoreSlim _semaphore = new(1);

    private readonly IAccountRepository _accountRepository;
    private readonly IDealRepository _dealRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IDerivativeRepository _derivativeRepository;

    public BcsReportJsonModelToEntitiesParser(
        IDealRepository dealRepository
        , IEventRepository eventRepository
        , IDerivativeRepository derivativeRepository
        , IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
        _dealRepository = dealRepository;
        _eventRepository = eventRepository;
        _derivativeRepository = derivativeRepository;
    }
    public Task HandleStepAsync(IEnumerable<DataAsJson> entities, CancellationToken cToken) =>
        Task.WhenAll(entities.Select(x => Task.Run(async () =>
        {
            try
            {
                var reportModel = x.Json.Deserialize<BcsReportModel>();

                if (reportModel!.Version != x.JsonVersion)
                    throw new Exception();

                var report = new BcsReportJsonSerializer(reportModel, x.UserId, _accountRepository, _derivativeRepository);

                var deals = report.GetDeals();
                var events = report.GetEvents();

                await _semaphore.WaitAsync(cToken);

                await _dealRepository.CreateRangeAsync(deals, cToken);
                await _eventRepository.CreateRangeAsync(events, cToken);

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
