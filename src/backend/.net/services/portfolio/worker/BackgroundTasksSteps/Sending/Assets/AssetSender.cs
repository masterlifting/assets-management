//using AM.Services.Portfolio.Core.Domain.Persistense.ProcessingEntities;

//using Shared.Background.Abstractions.EntitiesProcessing;

//namespace AM.Services.Portfolio.Worker.BackgroundTaskStepHandlers.StepImplementations.Sending.Assets;

//public sealed class AssetSender : IEntityProcessableStepHandler<Asset>
//{
//    //public AssetSender()
//    //{
//    //}

//    //public async Task StartAsync(IEnumerable<Derivative> derivatives, CancellationToken cToken)
//    //{
//    //    var derivativeArray = derivatives.ToArray();
//    //    var deals = await _unitOfWork.Deal.GetFullDealsAsync(derivativeArray);
//    //    var incomes = deals.Select(x => x.Income).ToArray();

//    //    foreach (var assetDerivatives in derivativeArray.GroupBy(x => (x.AssetId, x.AssetTypeId)))
//    //    {
//    //        var derivativeIds = assetDerivatives
//    //            .Select(x => (x.Id, x.Code))
//    //            .ToArray();

//    //        var dealIds = incomes
//    //            .Where(x => derivativeIds.Contains((x.DerivativeId,x.DerivativeCode)))
//    //            .Select(x => x.DealId)
//    //            .ToArray();

//    //        var assetDeals = deals.Where(x => dealIds.Contains(x.Id)).ToArray();

//    //        var dto = GetAssetDtoToRecommendations(assetDerivatives.Key.AssetId, assetDerivatives.Key.AssetTypeId, assetDerivatives.ToArray(), assetDeals);

//    //        var message = new AssetMqDto
//    //        {
//    //            Payload = dto.SerializeToString(),
//    //            Queue = new RabbitMqQueue
//    //            {
//    //                Name = "Recommendations",
//    //                IsAutoDelete = false,
//    //                IsDurable = false,
//    //                IsExclusive = false
//    //            },
//    //            Headers = new Dictionary<string, string>
//    //            {
//    //                {"Type", "AssetDto"},
//    //                {"Method", "Post"}
//    //            }
//    //        };

//    //        if (!_mqProducer.TryPublish(message, out var error))
//    //            _logger.LogError("RabbitMq", "AssetDto sending", new SharedSqlException("", "", error));
//    //    }
//    //}

//    //private static AssetDto GetAssetDtoToRecommendations(string assetId, int assetTypeId, IEnumerable<Derivative> assetDerivatives, Deal[] assetDeals)
//    //{
//    //    var assetBalance = assetDerivatives.Sum(y => y.Balance);
//    //    var (assetBalanceCost, assetLastDealCost) = Compute(assetBalance, assetDeals);
//    //    return new AssetDto(assetId, assetTypeId, assetBalance, assetBalanceCost, assetLastDealCost);
//    //}
//    //private static (decimal? balanceCost, decimal? lastDealCost) Compute(decimal assetBalance, Deal[] assetDeals)
//    //{
//    //    if (!assetDeals.Any())
//    //        return (null, null);

//    //    var lastDealCost = assetDeals.MaxBy(x => x.Date)!.Cost;

//    //    var expenseValue = 0m;
//    //    var assetBalanceCost = 0m;

//    //    foreach (var deal in assetDeals.OrderByDescending(x => x.Cost))
//    //    {
//    //        expenseValue += deal.Expense.Value;
//    //        var expenseValueTemp = deal.Expense.Value;

//    //        if (expenseValue > assetBalance)
//    //        {
//    //            expenseValueTemp = expenseValue - assetBalance;
//    //            expenseValue = assetBalance;
//    //        }

//    //        assetBalanceCost += deal.Cost * expenseValueTemp;

//    //        if (expenseValue == assetBalance)
//    //            break;
//    //    }

//    //    return (assetBalanceCost, lastDealCost);
//    //}

//    public Task HandleStepAsync(IEnumerable<Asset> entities, CancellationToken cToken)
//    {
//        throw new NotImplementedException();
//    }
//}