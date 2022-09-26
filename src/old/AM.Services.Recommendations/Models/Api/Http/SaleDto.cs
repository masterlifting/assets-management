using System;

namespace AM.Services.Recommendations.Models.Api.Http;

public record SaleDto
{
    public string Asset { get; init; } = null!;
    public SaleRecommendationDto[] Recommendations { get; init; } = Array.Empty<SaleRecommendationDto>();
}

public record SaleRecommendationDto
{
    public SaleRecommendationDto(decimal profitPlan, decimal? profitFact, decimal balance, decimal pricePlan, decimal? priceFact)
    {
        var _profitFact = "not computed";

        if (profitFact.HasValue)
        {
            var _pf = decimal.Round(profitFact.Value, 1);
            _profitFact = $"{_pf}%";
            if (_pf > 0)
                _profitFact = '+' + _profitFact;
        }

        ProfitFact = _profitFact;
        ProfitPlan = $"{decimal.Round(profitPlan, 1)}%";
        Balance = $"{balance:0.##########}";
        PricePlan = $"{pricePlan:0.##########}";
        PriceFact = $"{priceFact:0.##########}";
    }
    public string ProfitPlan { get; init; }
    public string ProfitFact { get; init; }
    public string Balance { get; init; }
    public string PricePlan { get; init; }
    public string PriceFact { get; init; }
}