namespace AM.Services.Common.Contracts.Models.RabbitMq.Api;

public record AssetRatingMqDto(string AssetId, byte AssetTypeId, int Place);