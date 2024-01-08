using AM.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Portfolio.Core.Abstractions.Services.DataHeapServices.Bcs.Transactions;
using AM.Portfolio.Core.Exceptions;
using AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs;
using AM.Portfolio.Core.Models.Services.DataHeapServices.Bcs.Transactions;
using AM.Portfolio.Core.Persistence.Entities.Sql;

using static AM.Portfolio.Core.Constants.Enums;
using static Net.Shared.Persistence.Models.Constants.Enums;

namespace AM.Portfolio.Core.Services.DataHeapServices.Bcs.Transactions;

public sealed class BcsTransactionsMapper : IBcsTransactionsMapper
{
    private const int HolderId = (int)Holders.Bcs;

    private static readonly Dictionary<string, Derivative> DerivativeMap = new();
    private static readonly Dictionary<(string, bool), int> EventTypeMap = new();

    private readonly List<Deal> _deals = Enumerable.Empty<Deal>().ToList();
    private readonly List<Event> _events = Enumerable.Empty<Event>().ToList();

    private readonly IDerivativeRepository _derivativeRepository;
    private readonly IDealRepository _dealRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICatalogRepository _catalogRepository;

    public BcsTransactionsMapper(
        IDerivativeRepository derivativeRepository,
        IDealRepository dealRepository,
        IEventRepository eventRepository,
        IAccountRepository accountRepository,
        ICatalogRepository catalogRepository)
    {
        _derivativeRepository = derivativeRepository;
        _dealRepository = dealRepository;
        _eventRepository = eventRepository;
        _accountRepository = accountRepository;
        _catalogRepository = catalogRepository;
    }

    public IReadOnlyCollection<Deal> Deals => _deals;
    public IReadOnlyCollection<Event> Events => _events;

    public async Task Map(BcsTransactionsResult result, CancellationToken cToken)
    {
        var account = await _accountRepository.Get(result.Agreement, HolderId, cToken);

        var derivatives = await _derivativeRepository.Get(cToken);

        for (var i = 0; i < derivatives.Length; i++)
        {
            var derivative = derivatives[i];

            DerivativeMap.TryAdd(derivative.Ticker, derivative);

            if (derivative.Code is not null)
                DerivativeMap.TryAdd(derivative.Code, derivative);
        }

        var eventTypes = await _catalogRepository.GetEventTypes(cToken);

        for (var i = 0; i < eventTypes.Length; i++)
        {
            var eventType = eventTypes[i];

            EventTypeMap.TryAdd((eventType.Name, eventType.IsIncreasable), eventType.Id);
        }

        var existingDealDates = await _dealRepository.GetDealDates(account, result, cToken);

        foreach (var bcsDeal in result.Deals.ExceptBy(existingDealDates, x => x.Income.Date))
            _deals.Add(CreateDeal(result.Source, account, bcsDeal));

        var existingEventDates = await _eventRepository.GetEventDates(account, result, cToken);

        foreach (var bcsEvent in result.Events.ExceptBy(existingEventDates, x => x.Date))
            _events.Add(CreateEvent(result.Source, account, bcsEvent));
    }

    private static Deal CreateDeal(string source, Account account, BcsDeal bcsDeal)
    {
        if (bcsDeal.Income.Date != bcsDeal.Expense.Date)
            throw new PortfolioCoreException("Income and expense dates of the deal was not equal.");

        var dealDate = bcsDeal.Income.Date;

        var incomeDerivative = GetDerivative(bcsDeal.Income.Asset);

        var expenseDerivative = GetDerivative(bcsDeal.Expense.Asset);

        var deal = new Deal()
        {
            Id = Guid.NewGuid(),
            UserId = account.UserId,
            DateTime = dealDate,
            Description = $"Source file: {source}",

            StepId = (int)ProcessSteps.None,
            StatusId = (int)ProcessStatuses.Draft,
        };

        deal.Income = new()
        {
            DealId = deal.Id,
            DateTime = dealDate,
            Value = bcsDeal.Income.Value,
            DerivativeId = incomeDerivative.Id,
            Description = bcsDeal.Income.Info,

            HolderId = HolderId,
            AccountId = account.Id,
            ExchangeId = (int)bcsDeal.Income.Exchange
        };

        deal.Expense = new()
        {
            DealId = deal.Id,
            DateTime = dealDate,
            Value = bcsDeal.Expense.Value,
            DerivativeId = expenseDerivative.Id,
            Description = bcsDeal.Expense.Info,

            HolderId = HolderId,
            AccountId = account.Id,
            ExchangeId = (int)bcsDeal.Expense.Exchange
        };

        return deal;
    }
    private static Event CreateEvent(string source, Account account, BcsEvent bcsEvent)
    {
        var (name, isIncreasable) = bcsEvent.Event switch
        {
            BcsReportEvents.Income => (EventTypes.TopUp, true),
            BcsReportEvents.Splitting => (EventTypes.Splitting, true),
            BcsReportEvents.Sharing => (EventTypes.Multiplying, true),
            BcsReportEvents.IncomeDividend or BcsReportEvents.IncomePercentage => (EventTypes.Percentage, true),

            BcsReportEvents.TaxZone => (EventTypes.Tax, false),
            BcsReportEvents.Expense => (EventTypes.Withdraw, false),
            BcsReportEvents.CommissionBroker or BcsReportEvents.CommissionDepositary => (EventTypes.Commission, false),

            _ => throw new NotImplementedException($"The event '{bcsEvent.Event}' is not recognized.")
        };

        if (!EventTypeMap.TryGetValue((name.ToString(), isIncreasable), out var eventTypeId))
            throw new PortfolioCoreException($"Event type '{name}' was not found.");

        var derivative = GetDerivative(bcsEvent.Asset);

        var stepId = bcsEvent.Event switch
        {
            BcsReportEvents.Splitting => (int)ProcessSteps.CalculateSplitting,
            _ => (int)ProcessSteps.CalculateBalance
        };

        return new()
        {
            TypeId = eventTypeId,
            DerivativeId = derivative.Id,
            Value = bcsEvent.Value,
            DateTime = bcsEvent.Date,
            Description = $"Source file '{source}'.",

            UserId = account.UserId,
            AccountId = account.Id,
            HolderId = HolderId,
            ExchangeId = (int)bcsEvent.Exchange,

            StatusId = (int)ProcessStatuses.Ready,
            StepId = stepId,
        };
    }

    private static Derivative GetDerivative(BcsAsset bcsAsset) =>
        !DerivativeMap.TryGetValue(bcsAsset.Ticker, out var derivative)
            ? bcsAsset.Code is not null
                ? DerivativeMap.TryGetValue(bcsAsset.Code, out derivative)
                    ? derivative
                    : throw new PortfolioCoreException($"Derivative '{bcsAsset.Ticker}' with code '{bcsAsset.Code}' was not found.")
                : throw new PortfolioCoreException($"Derivative '{bcsAsset.Ticker}' was not found.")
            : derivative;
}
