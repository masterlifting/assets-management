using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AM.Services.Common.Contracts.Helpers;
using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Portfolio.Domain.DataAccess;
using AM.Services.Portfolio.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace AM.Services.Portfolio.Services.Entity;

public class DealService
{
    public ILogger<DealService> Logger { get; }
    private readonly Repository<Deal> dealRepo;
    private readonly Repository<Income> incomeRepo;
    private readonly Repository<Expense> expenseRepo;
    private readonly Repository<Derivative> derivativeRepo;

    public DealService(
        ILogger<DealService> logger
        , Repository<Deal> dealRepo
        , Repository<Income> incomeRepo
        , Repository<Expense> expenseRepo
        , Repository<Derivative> derivativeRepo)
    {
        Logger = logger;
        this.dealRepo = dealRepo;
        this.incomeRepo = incomeRepo;
        this.expenseRepo = expenseRepo;
        this.derivativeRepo = derivativeRepo;
    }

    public Task SetAsync(QueueActions action, IReadOnlyCollection<Deal> entities) => action switch
    {
        QueueActions.Create => Up(entities),
        QueueActions.Delete => Down(entities),
        QueueActions.Update => Update(entities),
        _ => Logger.LogDefaultTask($"{action}")
    };
    public Task SetAsync(QueueActions action, Deal entity) => action switch
    {
        QueueActions.Create => Up(entity),
        QueueActions.Delete => Down(entity),
        QueueActions.Update => Update(entity),
        _ => Logger.LogDefaultTask($"{action}")
    };

    private async Task Up(Deal deal)
    {
        //var income = await incomeRepo.FindAsync(x => x.DealId == deal.Id);
        //if (income is null)
        //    throw new ApplicationException($"Deal '{deal.Id}' not complited. Income not found");

        //var expense = await expenseRepo.FindAsync(x => x.DealId == deal.Id);
        //if (expense is null)
        //    throw new ApplicationException($"Deal '{deal.Id}' not complited. Expense not found");

        //var derivatives = await derivativeRepo.GetSampleAsync(x =>
        //    new[] { income.DerivativeId, expense.DerivativeId }.Contains(x.Id)
        //    && new[] { income.DerivativeCode, expense.DerivativeCode }.Contains(x.Code));

        //var dicDerivatives = derivatives.ToDictionary(x => (x.Id, x.Code));

        //var incomeDerivative = dicDerivatives[(income.DerivativeId, income.DerivativeCode)];
        //var expenseDerivative = dicDerivatives[(expense.DerivativeId, expense.DerivativeCode)];

        //incomeDerivative.Balance += income.Value;
        //expenseDerivative.Balance -= expense.Value;

        //await derivativeRepo.UpdateRangeAsync(new[] { incomeDerivative, expenseDerivative }, "up balances");
    }
    private async Task Down(Deal deal)
    {
        
    }
    private async Task Update(Deal deal)
    {
      
    }

    private async Task Up(IEnumerable<Deal> deals)
    {
        deals = deals.ToArray();

        var derivativeIds = deals.SelectMany(x => new[]{ x.Income.DerivativeId, x.Expense.DerivativeId}).Distinct();
        var derivativeCodes = deals.SelectMany(x => new[] { x.Income.DerivativeCode, x.Expense.DerivativeCode }).Distinct();
        var derivatives = await derivativeRepo.GetSampleAsync(x => derivativeIds.Contains(x.Id) && derivativeCodes.Contains(x.Code));
        var dicDerivatives = derivatives.ToDictionary(x => (x.Id, x.Code));
        
        foreach (var derivative in dicDerivatives)
            derivative.Value.Balance = 0;

        foreach (var deal in deals)
        {
            dicDerivatives[(deal.Income.DerivativeId, deal.Income.DerivativeCode)].Balance += deal.Income.Value;
            dicDerivatives[(deal.Expense.DerivativeId, deal.Expense.DerivativeCode)].Balance -= deal.Expense.Value;
        }

        await derivativeRepo.UpdateRangeAsync(derivatives, "up balances");
    }
    private async Task Down(IEnumerable<Deal> deals)
    {
        
    }
    private async Task Update(IEnumerable<Deal> deals)
    {
        
    }
}