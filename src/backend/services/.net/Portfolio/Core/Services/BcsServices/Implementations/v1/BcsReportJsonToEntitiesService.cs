using AM.Services.Portfolio.Core.Domain.Persistense.Collections.BcsReport;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Services.BcsServices.Interfaces;

using Shared.Persistense.Abstractions.Repositories;

using System.Globalization;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;

namespace AM.Services.Portfolio.Core.Services.BcsServices.Implementations.v1;

public sealed class BcsReportJsonToEntitiesService : IBcsReportJsonToEntitiesService
{
    private readonly IFormatProvider _culture = new CultureInfo("ru-RU");
    //private readonly ProviderId _providerId = new(Providers.Bcs);
    private readonly IPostgreSQLRepository _repository;

    public BcsReportJsonToEntitiesService(IPostgreSQLRepository repository) => _repository = repository;

    public Event[] GetEvents(BcsReportModel reportModel)
    {
        //var eventModels = reportModel.Events.ToArray();

        //var result = new List<Event>(eventModels.Length);

        //foreach (var item in eventModels)
        //{
        //    var derivativeId = new DerivativeId(item.Asset);
        //    var derivativeCode = new DerivativeCode(_derivativeDictionary[derivativeId.AsString][0]);

        //    var eventTypeId = new EventTypeId(item.EventType);
        //    var exchangeId = new ExchangeId(item.Exchange);

        //    var model = new EventModel(
        //        decimal.Parse(item.Value, _culture)
        //        , DateOnly.Parse(item.Date, _culture)
        //        , eventTypeId
        //        , derivativeId
        //        , derivativeCode
        //        , _accountId
        //        , _userId
        //        , _providerId
        //        , exchangeId
        //        , new StateId(Shared.Persistense.Constants.Enums.Statuses.Ready)
        //        , new StepId(Shared.Persistense.Constants.Enums.Steps.Computing)
        //        , 0
        //        , item.Info);

        //    result.Add(model);
        //}

        return Array.Empty<Event>(); //result.ToArray();
    }
    public Deal[] GetDeals(BcsReportModel reportModel)
    {
        //var dealModels = reportModel.Deals.ToArray();

        //var result = new List<Deal>(dealModels.Length);

        //foreach (var item in dealModels)
        //{
        //    var derivativeId = new DerivativeId(item.Asset);
        //    var derivativeCode = new DerivativeCode(_derivativeDictionary[derivativeId.AsString][0]);

        //    var exchangeId = new ExchangeId(item.Exchange);

        //    var cost = decimal.Parse(item.Cost, _culture);
        //    var value = decimal.Parse(item.Value, _culture);

        //    var date = DateOnly.Parse(item.Date, _culture);

        //    var dealId = new DealId();

        //    var incomederivativeId = new DerivativeId(item.Asset);
        //    var incomederivativeCode = new DerivativeCode(_derivativeDictionary[derivativeId.AsString][0]);


        //    var income = new IncomeModel(dealId, derivativeId, derivativeCode, cost, date);

        //    var expensederivativeId = new DerivativeId(item.Asset);
        //    var expensederivativeCode = new DerivativeCode(_derivativeDictionary[derivativeId.AsString][0]);


        //    var expense = new ExpenseModel(dealId, derivativeId, derivativeCode, cost, date);

        //    var model = new DealModel(
        //        cost * value
        //        , date
        //        , income
        //        , expense
        //        , _accountId
        //        , _userId
        //        , _providerId
        //        , exchangeId
        //        , new StateId(Shared.Persistense.Constants.Enums.Statuses.Ready)
        //        , new StepId(Shared.Persistense.Constants.Enums.Steps.Computing)
        //        , 0
        //        , null);

        //    result.Add(model);
        //}

        //return result.ToArray();

        return Array.Empty<Deal>();
    }
}