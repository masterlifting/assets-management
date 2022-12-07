using System;

namespace AM.Services.Recommendations.Models.Api.Http;

public record PurchaseDto
{
    public string Asset { get; init; } = null!;
    public PurchaseRecommendationDto[] Recommendations { get; init; } = Array.Empty<PurchaseRecommendationDto>();
}

public record PurchaseRecommendationDto
{
    public PurchaseRecommendationDto(decimal discountPlan, decimal? discountFact, decimal pricePlan, decimal priceFact, decimal? prceNext)
    {
        var _discountFact = "not computed";

        if (discountFact.HasValue)
        {
            var _df = decimal.Round(discountFact.Value, 1);
            _discountFact = $"{_df}%";
            if (_df > 0)
                _discountFact = '+' + _discountFact;
        }

        DiscountPlan = $"{decimal.Round(discountPlan, 1)}%";
        DiscountFact = _discountFact;
        PricePlan = $"{pricePlan:0.##########}";
        PriceFact = $"{priceFact:0.##########}";
        PriceNext = prceNext.HasValue ? $"{prceNext.Value:0.##########}" : "not found";
    }
    public string DiscountPlan { get; init; }
    public string DiscountFact { get; init; }
    public string PricePlan { get; init; }
    public string PriceFact { get; init; }
    public string PriceNext { get; init; }
}