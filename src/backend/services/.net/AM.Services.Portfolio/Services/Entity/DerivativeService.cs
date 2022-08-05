using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AM.Services.Common.Contracts.Models.RabbitMq.Api;
using AM.Services.Portfolio.Domain.DataAccess;
using AM.Services.Portfolio.Domain.Entities;
using Microsoft.Extensions.Logging;
using static AM.Services.Common.Contracts.Enums;

namespace AM.Services.Portfolio.Services.Entity;

public class DerivativeService
{
    public ILogger<DealService> Logger { get; }
    private readonly Repository<Deal> dealRepo;
    private readonly Repository<Income> incomeRepo;

    public DerivativeService(
        ILogger<DealService> logger
        , Repository<Deal> dealRepo
        , Repository<Income> incomeRepo)
    {
        Logger = logger;
        this.dealRepo = dealRepo;
        this.incomeRepo = incomeRepo;
    }

    public Task<AssetPortfolioMqDto[]> GetTransferModelsAsync(IEnumerable<Derivative> entities) => GetModels(entities.ToArray());
    public Task<AssetPortfolioMqDto> GetTransferModelAsync(Derivative entity) => GetModel(entity);

    private async Task<AssetPortfolioMqDto[]> GetModels(Derivative[] derivatives)
    {
        derivatives = derivatives.Where(x => x.AssetTypeId == (byte) AssetTypes.Stock).ToArray();

        var derivativeIds = derivatives.Select(x => x.Id);
        var derivativeCodes = derivatives.Select(x => x.Code);

        var incomes = await incomeRepo.GetSampleAsync(x => derivativeIds.Contains(x.DerivativeId) && derivativeCodes.Contains(x.DerivativeCode));
        var dealIds = incomes.Select(x => x.DealId);
        var deals = await dealRepo.GetSampleAsync(x => dealIds.Contains(x.Id));

        return derivatives
            .GroupBy(x => (x.AssetId, x.AssetTypeId))
            .Select(x =>
            {
                var balance = x.Sum(y => y.Balance);
                var _incomes = incomes
                    .Join(x, y => (y.DerivativeId, y.DerivativeCode), z => (z.Id, z.Code), (y,_) => y)
                    .ToArray();
                var _deals = deals.Join(_incomes, y => y.Id, z => z.DealId, (y, _) => y).ToArray();

                var (balanceCost, lastDealPrice) = Compute(balance, _deals, _incomes);

                return new AssetPortfolioMqDto(x.Key.AssetId, x.Key.AssetTypeId, balance, balanceCost, lastDealPrice);
            })
            .ToArray();
    }
    private async Task<AssetPortfolioMqDto> GetModel(Derivative derivative)
    {
        var incomes = await incomeRepo.GetSampleAsync(x => derivative.Id == x.DerivativeId && derivative.Code == x.DerivativeCode);
        var dealIds = incomes.Select(x => x.DealId);
        var deals = await dealRepo.GetSampleAsync(x => dealIds.Contains(x.Id));
        var (balanceCost, lastDealPrice) = Compute(derivative.Balance, deals, incomes);
        return new(derivative.AssetId, derivative.AssetTypeId, derivative.Balance, balanceCost, lastDealPrice);
    }

    private static (decimal? balanceCost, decimal? lastDealPrice) Compute(decimal balance, Deal[] deals, IEnumerable<Income> incomes)
    {
        if(!deals.Any())
            return (null, null);

        var _lastDealPrice = deals.MaxBy(x => x.Date)!.Cost;
        
        var _balance = 0m;
        var _balanceCost = 0m;

        foreach (var item in deals
                     .Join(incomes, x => x.Id, y => y.DealId, (x, y) => (Deal: x, Expense: y))
                     .OrderByDescending(x => x.Deal.Cost))
        {
            _balance += item.Expense.Value;
            var expenseValue = item.Expense.Value;

            if (_balance > balance)
            {
                expenseValue = _balance - balance;
                _balance = balance;
            }

            _balanceCost += item.Deal.Cost * expenseValue;

            if (balance == _balance)
                break;
        }

        return (_balanceCost, _lastDealPrice);
    }
}