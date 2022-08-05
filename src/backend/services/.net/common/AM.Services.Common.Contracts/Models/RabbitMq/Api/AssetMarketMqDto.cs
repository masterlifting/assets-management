namespace AM.Services.Common.Contracts.Models.RabbitMq.Api;

public record AssetMarketMqDto(string AssetId, byte AssetTypeId, decimal PriceActual, decimal PriceAvg);