using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Core.Domain.Persistense.Models;
using AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;
using AM.Services.Portfolio.Core.Services.EntityState.Steps.Parsing.ReportsData.Bcs.Models;

using Microsoft.Extensions.Logging;

using Shared.Persistense.Models.ValueObject.EntityState;

using System.Globalization;

using static AM.Services.Common.Contracts.Constants.Persistense.Enums;
using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Services.EntityState.Steps.Serialization.ReportsData.Bcs;

public sealed class BcsReport
{
    private readonly ProviderId _providerId = new(Providers.Bcs);
    private readonly IFormatProvider _culture = new CultureInfo("ru-RU");

    private readonly ILogger _logger;
    private readonly BcsReportModel _reportModel;

    private readonly IDictionary<string, string[]> _derivativeDictionary;

    public ReportData File { get; }

    public UserId UserId { get; }
    public AccountId AccountId { get; }

    public DateOnly DateStart { get; }
    public DateOnly DateEnd { get; }

    public IEnumerable<Event> Events { get; }
    private readonly List<Event> _events;
    public IEnumerable<Deal> Deals { get; }
    private readonly List<Deal> _deals;

    public BcsReport(
        ILogger logger
        , ReportData file
        , UserId userId
        , BcsReportModel reportModel
        , IDictionary<string, int> accountDictionary
        , IDictionary<string, string[]> derivativeDictionary)
    {
        _logger = logger;
        File = file;
        _reportModel = reportModel;
        _derivativeDictionary = derivativeDictionary;

        UserId = userId;
        AccountId = new AccountId(reportModel.Agreement, accountDictionary);
        DateStart = DateOnly.Parse(reportModel.DateStart, _culture);
        DateEnd = DateOnly.Parse(reportModel.DateEnd, _culture);

        _events = new List<Event>();
        _deals = new List<Deal>();

        Events = _events;
        Deals = _deals;
    }


    public void SetEvents()
    {
        if (_reportModel.Dividends is not null)
        {
            var models = GetEventModels(_reportModel.Dividends.ToArray());
            _events.AddRange(models.Select(x => x.GetEntity()));
        }
        if (_reportModel.Comissions is not null)
        {
            var models = GetEventModels(_reportModel.Comissions.ToArray());
            _events.AddRange(models.Select(x => x.GetEntity()));
        }
        if (_reportModel.Balances is not null)
        {
            var models = GetEventModels(_reportModel.Balances.ToArray());
            _events.AddRange(models.Select(x => x.GetEntity()));
        }
        if (_reportModel.StockMoves is not null)
        {
            var models = GetEventModels(_reportModel.StockMoves.ToArray());
            _events.AddRange(models.Select(x => x.GetEntity()));
        }
    }
    public void SetDeals()
    {
        if (_reportModel.ExchangeRates is not null)
        {
            var models = GetEventModels(_reportModel.ExchangeRates.ToArray());
            _deals.AddRange(models.Select(x => x.GetEntity()));
        }
        if (_reportModel.Transactions is not null)
        {
            var models = GetEventModels(_reportModel.Transactions.ToArray());
            _deals.AddRange(models.Select(x => x.GetEntity()));
        }
    }

    private EventModel[] GetEventModels(BcsReportDividendModel[] items)
    {
        var result = new List<EventModel>(items.Length);

        foreach (var item in items)
        {
            var derivativeId = new DerivativeId(item.Currency);
            var derivativeCode = new DerivativeCode(_derivativeDictionary[derivativeId.AsString][0]);

            var eventTypeId = new EventTypeId(item.EventType, new Dictionary<string, int>
            {
                { "1", (int)EventTypes.Dividend },
                { "2", (int)EventTypes.TaxIncome }
            });
            var exchangeId = new ExchangeId(item.Exchange, new Dictionary<string, int>
            {
                { "1", (int)Exchanges.Spbex },
                { "2", (int)Exchanges.Moex }
            });

            var model = new EventModel(
                decimal.Parse(item.Sum, _culture)
                , DateOnly.Parse(item.Date, _culture)
                , eventTypeId
                , derivativeId
                , derivativeCode
                , AccountId
                , UserId
                , _providerId
                , exchangeId
                , new StateId(Shared.Persistense.Constants.Enums.States.Ready)
                , new StepId(Shared.Persistense.Constants.Enums.Steps.Computing)
                , 0
                , item.Info);

            result.Add(model);
        }

        return result.ToArray();
    }
    private EventModel[] GetEventModels(BcsReportComissionModel[] items)
    {
        var result = new List<EventModel>(items.Length);

        foreach (var item in items)
        {
            var derivativeId = new DerivativeId(item.Currency);
            var derivativeCode = new DerivativeCode(_derivativeDictionary[derivativeId.AsString][0]);

            var eventTypeId = new EventTypeId(item.EventType, new Dictionary<string, int>
            {
                { "1", (int)EventTypes.TaxCountry },
                { "2", (int)EventTypes.TaxDepositary },
                { "3", (int)EventTypes.TaxDeal },
                { "4", (int)EventTypes.TaxIncome }
            });
            var exchangeId = new ExchangeId(item.Exchange, new Dictionary<string, int>
            {
                { "1", (int)Exchanges.Spbex },
                { "2", (int)Exchanges.Moex }
            });

            var model = new EventModel(
                decimal.Parse(item.Sum, _culture)
                , DateOnly.Parse(item.Date, _culture)
                , eventTypeId
                , derivativeId
                , derivativeCode
                , AccountId
                , UserId
                , _providerId
                , exchangeId
                , new StateId(Shared.Persistense.Constants.Enums.States.Ready)
                , new StepId(Shared.Persistense.Constants.Enums.Steps.Computing)
                , 0
                , null);

            result.Add(model);
        }

        return result.ToArray();
    }
    private EventModel[] GetEventModels(BcsReportBalanceModel[] items)
    {
        var result = new List<EventModel>(items.Length);

        foreach (var item in items)
        {
            var derivativeId = new DerivativeId(item.Currency);
            var derivativeCode = new DerivativeCode(_derivativeDictionary[derivativeId.AsString][0]);

            var eventTypeId = new EventTypeId(item.EventType, new Dictionary<string, int>
            {
                { "1", (int)EventTypes.Dividend },
                { "2", (int)EventTypes.TaxIncome }
            });
            var exchangeId = new ExchangeId(item.Exchange, new Dictionary<string, int>
            {
                { "1", (int)Exchanges.Spbex },
                { "2", (int)Exchanges.Moex }
            });

            var model = new EventModel(
                decimal.Parse(item.Sum, _culture)
                , DateOnly.Parse(item.Date, _culture)
                , eventTypeId
                , derivativeId
                , derivativeCode
                , AccountId
                , UserId
                , _providerId
                , exchangeId
                , new StateId(Shared.Persistense.Constants.Enums.States.Ready)
                , new StepId(Shared.Persistense.Constants.Enums.Steps.Computing)
                , 0
                , null);

            result.Add(model);
        }

        return result.ToArray();
    }
    private EventModel[] GetEventModels(BcsReportStockActionModel[] items)
    {
        var result = new List<EventModel>(items.Length);

        foreach (var item in items)
        {
            var derivativeId = new DerivativeId(item.Ticker);
            var derivativeCode = new DerivativeCode(_derivativeDictionary[derivativeId.AsString][0]);

            var eventTypeId = new EventTypeId(item.Info, new Dictionary<string, int>
            {
                { "1", (int)EventTypes.Dividend },
                { "2", (int)EventTypes.TaxIncome }
            });
            var exchangeId = new ExchangeId((int)Exchanges.Spbex);

            var model = new EventModel(
                decimal.Parse(item.Value, _culture)
                , DateOnly.Parse(item.Date, _culture)
                , eventTypeId
                , derivativeId
                , derivativeCode
                , AccountId
                , UserId
                , _providerId
                , exchangeId
                , new StateId(Shared.Persistense.Constants.Enums.States.Ready)
                , new StepId(Shared.Persistense.Constants.Enums.Steps.Computing)
                , 0
                , item.Info);

            result.Add(model);
        }

        return result.ToArray();
    }
    
    private DealModel[] GetEventModels(BcsReportExchangeRateModel[] items)
    {
        var result = new List<DealModel>(items.Length);

        foreach (var item in items)
        {
            var derivativeId = new DerivativeId(item.Currency);
            var derivativeCode = new DerivativeCode(_derivativeDictionary[derivativeId.AsString][0]);

            var eventTypeId = new EventTypeId(item.EventType, new Dictionary<string, int>
            {
                { "1", (int)EventTypes.Dividend },
                { "2", (int)EventTypes.TaxIncome }
            });
            var exchangeId = new ExchangeId(item.Exchange, new Dictionary<string, int>
            {
                { "1", (int)Exchanges.Spbex },
                { "2", (int)Exchanges.Moex }
            });

            var value = decimal.Parse(item.Sum, _culture);
            var date = DateOnly.Parse(item.Date, _culture);

            var dealId = new DealId();

            var income = new IncomeModel(dealId, derivativeId, derivativeCode, value, date);
            var expense = new ExpenseModel(dealId, derivativeId, derivativeCode, value, date);

            var model = new DealModel(
                value
                , date
                , income
                , expense
                , AccountId
                , UserId
                , _providerId
                , exchangeId
                , new StateId(Shared.Persistense.Constants.Enums.States.Ready)
                , new StepId(Shared.Persistense.Constants.Enums.Steps.Computing)
                , 0
                , null);

            result.Add(model);
        }

        return result.ToArray();
    }
    private DealModel[] GetEventModels(BcsReportTransactionModel[] items)
    {
        var result = new List<DealModel>(items.Length);

        foreach (var item in items)
        {
            var derivativeId = new DerivativeId(item.Currency);
            var derivativeCode = new DerivativeCode(_derivativeDictionary[derivativeId.AsString][0]);

            var eventTypeId = new EventTypeId(item.EventType, new Dictionary<string, int>
            {
                { "1", (int)EventTypes.Dividend },
                { "2", (int)EventTypes.TaxIncome }
            });
            var exchangeId = new ExchangeId(item.Exchange, new Dictionary<string, int>
            {
                { "1", (int)Exchanges.Spbex },
                { "2", (int)Exchanges.Moex }
            });

            var value = decimal.Parse(item.Sum, _culture);
            var date = DateOnly.Parse(item.Date, _culture);

            var dealId = new DealId();

            var income = new IncomeModel(dealId, derivativeId, derivativeCode, value, date);
            var expense = new ExpenseModel(dealId, derivativeId, derivativeCode, value, date);

            var model = new DealModel(
                value
                , date
                , income
                , expense
                , AccountId
                , UserId
                , _providerId
                , exchangeId
                , new StateId(Shared.Persistense.Constants.Enums.States.Ready)
                , new StepId(Shared.Persistense.Constants.Enums.Steps.Computing)
                , 0
                , null);

            result.Add(model);
        }

        return result.ToArray();
    }
}