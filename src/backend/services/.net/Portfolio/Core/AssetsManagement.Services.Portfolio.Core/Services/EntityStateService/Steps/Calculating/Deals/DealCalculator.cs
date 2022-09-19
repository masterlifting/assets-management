﻿using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

using Microsoft.EntityFrameworkCore;

using Shared.Persistense.Abstractions.Entities.State.Handle;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Calculating.Deals;

public class DealCalculator : IEntityStateStepHandler<Deal>
{
    //public DealCalculator()
    //{

    //}

    //public async Task CalculateAsync(IEnumerable<Deal> deals, CancellationToken cToken)
    //{
    //    deals = deals.ToArray();

    //    var derivativeIds = deals.SelectMany(x => new[] { x.Income.DerivativeId, x.Expense.DerivativeId }).Distinct();
    //    var derivativeCodes = deals.SelectMany(x => new[] { x.Income.DerivativeCode, x.Expense.DerivativeCode }).Distinct();
    //    var derivatives = await _unitOfWork.Derivative.DbSet
    //        .Where(x => derivativeIds.Contains(x.Id) && derivativeCodes.Contains(x.Code))
    //        .ToArrayAsync();

    //    var dicDerivatives = derivatives.ToDictionary(x => (x.Id, x.Code));

    //    foreach (var derivative in dicDerivatives)
    //        derivative.Value.Balance = 0;

    //    foreach (var deal in deals)
    //    {
    //        dicDerivatives[(deal.Income.DerivativeId, deal.Income.DerivativeCode)].Balance += deal.Income.Value;
    //        dicDerivatives[(deal.Expense.DerivativeId, deal.Expense.DerivativeCode)].Balance -= deal.Expense.Value;
    //    }

    //    await _unitOfWork.Derivative.UpdateRangeAsync(derivatives, CancellationToken.None);
    //}

    public Task HandleAsync(IEnumerable<Deal> entities, CancellationToken cToken)
    {
        throw new NotImplementedException();
    }
}