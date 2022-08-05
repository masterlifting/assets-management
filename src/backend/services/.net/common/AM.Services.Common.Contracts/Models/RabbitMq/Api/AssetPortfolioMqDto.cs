namespace AM.Services.Common.Contracts.Models.RabbitMq.Api;

public record AssetPortfolioMqDto(string AssetId, byte AssetTypeId, decimal Balance, decimal? BalanceCost, decimal? LastDealCost);