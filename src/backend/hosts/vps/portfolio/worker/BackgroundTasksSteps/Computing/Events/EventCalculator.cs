//using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;

//using Shared.Background.Abstractions.EntitiesProcessing;

//namespace AM.Services.Portfolio.Worker.BackgroundTaskStepHandlers.StepImplementations.Computing.Events;

//public sealed class EventCalculator : IEntityProcessableStepHandler<Event>
//{
//    //private readonly Dictionary<OperationTypes, byte[]> _operationTypes;

//    //public EventCalculator()
//    //{
//    //    _operationTypes = new Dictionary<OperationTypes, byte[]>
//    //    {
//    //        {OperationTypes.Income, eventTypeRepo.Where(x => x.OperationTypeId == (byte)OperationTypes.Income).Select(x => x.Id).ToArray()},
//    //        {OperationTypes.Expense, eventTypeRepo.Where(x => x.OperationTypeId == (byte)OperationTypes.Expense).Select(x => x.Id).ToArray()}
//    //    };
//    //}

//    //public Task SetDerivativeBalancesAsync(QueueActions action, IEnumerable<Event> entities) => action switch
//    //{
//    //    QueueActions.Create => Up(entities),
//    //    QueueActions.Delete => Down(entities),
//    //    QueueActions.Update => Update(entities),
//    //    _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
//    //};
//    //public Task SetDerivativeBalanceAsync(QueueActions action, Event entity) => action switch
//    //{
//    //    QueueActions.Create => Up(entity),
//    //    QueueActions.Delete => Down(entity),
//    //    QueueActions.Update => Update(entity),
//    //    _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
//    //};

//    //private async Task Up(Event @event)
//    //{
//    //    var derivative = await _derivativeRepo.FindAsync(@event.DerivativeId, @event.DerivativeCode);

//    //    if (derivative is null)
//    //        throw new ApplicationException($"Derivative '{@event.DerivativeCode}' not found");

//    //    derivative.Balance += Compute(@event);

//    //    await _derivativeRepo.UpdateAsync(new object[] { @event.DerivativeId, @event.DerivativeCode }, derivative, "up balance");
//    //}
//    //private async Task Down(Event @event)
//    //{
//    //    var derivative = await _derivativeRepo.FindAsync(@event.DerivativeId, @event.DerivativeCode);

//    //    if (derivative is null)
//    //        throw new ApplicationException($"Derivative '{@event.DerivativeCode}' not found");

//    //    derivative.Balance += Compute(@event);

//    //    await _derivativeRepo.UpdateAsync(new object[] { @event.DerivativeId, @event.DerivativeCode }, derivative, "down balance");
//    //}
//    //private async Task Update(Event @event)
//    //{
//    //    var derivative = await _derivativeRepo.FindAsync(@event.DerivativeId, @event.DerivativeCode);

//    //    if (derivative is null)
//    //        throw new ApplicationException($"Derivative '{@event.DerivativeCode}' not found");

//    //    var events = await _eventRepo.GetSampleAsync(x =>
//    //        x.DerivativeId == @event.DerivativeId
//    //        && x.DerivativeCode == @event.DerivativeCode
//    //        && x.AccountId == @event.AccountId
//    //        && x.UserId == @event.UserId
//    //        && x.ProviderId == @event.ProviderId
//    //        && x.ExchangeId == @event.ExchangeId);

//    //    var balance = Compute(events);

//    //    derivative.Balance = balance;

//    //    await _derivativeRepo.UpdateAsync(new object[] { @event.DerivativeId, @event.DerivativeCode }, derivative, "update balance");
//    //}

//    //private async Task Up(IEnumerable<Event> events)
//    //{
//    //    events = events.ToArray();

//    //    var groupedEvents = events
//    //        .GroupBy(x => (x.DerivativeId, x.DerivativeCode))
//    //        .ToDictionary(x => x.Key, y => Compute(y.ToArray()));

//    //    var derivativeIds = events.Select(x => x.DerivativeId).Distinct();
//    //    var derivativeCodes = events.Select(x => x.DerivativeCode).Distinct().ToArray();

//    //    var derivatives = await _derivativeRepo.GetSampleAsync(x => derivativeIds.Contains(x.Id) && derivativeCodes.Contains(x.Code));
//    //    var dicDerivatives = derivatives.ToDictionary(x => (x.Id, x.Code));
//    //    foreach (var derivative in dicDerivatives)
//    //        derivative.Value.Balance = 0;

//    //    foreach (var (key, sum) in groupedEvents)
//    //        dicDerivatives[key].Balance += sum;

//    //    await _derivativeRepo.UpdateRangeAsync(derivatives, "up balances");
//    //}
//    //private async Task Down(IEnumerable<Event> events)
//    //{
//    //    events = events.ToArray();

//    //    var groupedEvents = events
//    //        .GroupBy(x => (x.DerivativeId, x.DerivativeCode))
//    //        .ToDictionary(x => x.Key, y => Compute(y.ToArray()));

//    //    var derivativeIds = events.Select(x => x.DerivativeId).Distinct();
//    //    var derivativeCodes = events.Select(x => x.DerivativeCode).Distinct();

//    //    var derivatives = await _derivativeRepo.GetSampleAsync(x => derivativeIds.Contains(x.Id) && derivativeCodes.Contains(x.Code));
//    //    var dicDerivatives = derivatives.ToDictionary(x => (x.Id, x.Code));

//    //    var result = new List<Derivative>(groupedEvents.Count);
//    //    foreach (var (key, sum) in groupedEvents)
//    //        if (dicDerivatives.ContainsKey(key))
//    //        {
//    //            var derivative = dicDerivatives[key];
//    //            derivative.Balance += sum;
//    //            result.Add(derivative);
//    //        }
//    //        else
//    //            Logger<>.LogWarning("Up derivative balances", key.DerivativeCode, "not found");

//    //    await _derivativeRepo.UpdateRangeAsync(result, "down balances");
//    //}
//    //private async Task Update(IEnumerable<Event> events)
//    //{
//    //    events = events.ToArray();

//    //    var dbEvents = await _eventRepo.GetExistAsync(events);

//    //    var groupedEvents = dbEvents
//    //        .GroupBy(x => (x.DerivativeId, x.DerivativeCode))
//    //        .ToDictionary(x => x.Key, y => Compute(y.ToArray()));

//    //    var derivativeIds = events.Select(x => x.DerivativeId).Distinct();
//    //    var derivativeCodes = events.Select(x => x.DerivativeCode).Distinct();

//    //    var derivatives = await _derivativeRepo.GetSampleAsync(x => derivativeIds.Contains(x.Id) && derivativeCodes.Contains(x.Code));
//    //    var dicDerivatives = derivatives.ToDictionary(x => (x.Id, x.Code));

//    //    var result = new List<Derivative>(groupedEvents.Count);
//    //    foreach (var (key, sum) in groupedEvents)
//    //        if (dicDerivatives.ContainsKey(key))
//    //        {
//    //            var derivative = dicDerivatives[key];
//    //            derivative.Balance = sum;
//    //            result.Add(derivative);
//    //        }
//    //        else
//    //            Logger<>.LogWarning("Update derivative balances", key.DerivativeCode, "not found");

//    //    await _derivativeRepo.UpdateRangeAsync(result, "update balances");
//    //}

//    //private decimal Compute(Event @event) => _operationTypes[OperationTypes.Income].Contains(@event.EventTypeId)
//    //    ? @event.Value
//    //    : _operationTypes[OperationTypes.Expense].Contains(@event.EventTypeId)
//    //        ? @event.Value
//    //        : throw new ApplicationException($"Derivative '{@event.DerivativeCode}' balance not computed");
//    //private decimal Compute(Event[] events)
//    //{
//    //    var income = events.Where(x => _operationTypes[OperationTypes.Income].Contains(x.EventTypeId)).ToArray();
//    //    var expense = events.Where(x => _operationTypes[OperationTypes.Expense].Contains(x.EventTypeId)).ToArray();

//    //    return income.Sum(x => x.Value) - expense.Sum(x => x.Value);
//    //}

//    public Task HandleStepAsync(IEnumerable<Event> entities, CancellationToken cToken)
//    {
//        throw new NotImplementedException();
//    }
//}