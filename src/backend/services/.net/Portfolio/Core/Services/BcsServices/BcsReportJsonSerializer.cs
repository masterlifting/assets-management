using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.EntityModels;
using AM.Services.Portfolio.Core.Domain.EntityStateModels.Report.Bcs;
using AM.Services.Portfolio.Core.Domain.EntityValueObjects;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Exceptions;

using Shared.Persistense.Abstractions.Repositories;

using System.Globalization;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Services.BcsServices;

public sealed class BcsReportJsonSerializer
{
    private readonly ProviderId _providerId = new(Providers.Bcs);
    private readonly IFormatProvider _culture = new CultureInfo("ru-RU");

    private readonly BcsReportModel _reportModel;

    private readonly IDictionary<string, string[]> _derivativeDictionary;

    public Guid UserId { get; }
    public int AccountId { get; }

    public DateOnly DateStart { get; }
    public DateOnly DateEnd { get; }

    public BcsReportJsonSerializer(
        BcsReportModel reportModel
        , Guid userId
        , IRepository repository)
        {
            _reportModel = reportModel;

            UserId = userId;

        var a = repository.

            if (!accountDictionary.ContainsKey(reportModel.Agreement))
                throw new PortfolioCoreException(nameof(AccountId), Actions.ValueObject.Validate, new(Actions.ValueObject.ValueNotValidError(value)));

            AccountId = accountDictionary[reportModel.Agreement];

            //DateStart = DateOnly.Parse(reportModel.DateStart, _culture);
            //DateEnd = DateOnly.Parse(reportModel.DateEnd, _culture);
        }


    public Event[] GetEvents()
    {
        var eventModels = _reportModel.Events.ToArray();

        var result = new List<Event>(eventModels.Length);

        foreach (var item in eventModels)
        {
            var derivativeId = new DerivativeId(item.Asset);
            var derivativeCode = new DerivativeCode(_derivativeDictionary[derivativeId.AsString][0]);

            var eventTypeId = new EventTypeId(item.EventType);
            var exchangeId = new ExchangeId(item.Exchange);

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
                , new StateId(Shared.Persistense.Constants.Enums.Statuses.Ready)
                , new StepId(Shared.Persistense.Constants.Enums.Steps.Computing)
                , 0
                , item.Info);

            result.Add(model);
        }

        return result.ToArray();
    }
    public Deal[] GetDeals()
    {
        var dealModels = _reportModel.Deals.ToArray();

        var result = new List<Deal>(dealModels.Length);

        foreach (var item in dealModels)
        {
            var derivativeId = new DerivativeId(item.Asset);
            var derivativeCode = new DerivativeCode(_derivativeDictionary[derivativeId.AsString][0]);

            var exchangeId = new ExchangeId(item.Exchange);

            var cost = decimal.Parse(item.Cost, _culture);
            var value = decimal.Parse(item.Value, _culture);

            var date = DateOnly.Parse(item.Date, _culture);

            var dealId = new DealId();

            var incomederivativeId = new DerivativeId(item.Asset);
            var incomederivativeCode = new DerivativeCode(_derivativeDictionary[derivativeId.AsString][0]);


            var income = new IncomeModel(dealId, derivativeId, derivativeCode, cost, date);

            var expensederivativeId = new DerivativeId(item.Asset);
            var expensederivativeCode = new DerivativeCode(_derivativeDictionary[derivativeId.AsString][0]);


            var expense = new ExpenseModel(dealId, derivativeId, derivativeCode, cost, date);

            var model = new DealModel(
                cost * value
                , date
                , income
                , expense
                , AccountId
                , UserId
                , _providerId
                , exchangeId
                , new StateId(Shared.Persistense.Constants.Enums.Statuses.Ready)
                , new StepId(Shared.Persistense.Constants.Enums.Steps.Computing)
                , 0
                , null);

            result.Add(model);
        }

        return result.ToArray();
    }
}