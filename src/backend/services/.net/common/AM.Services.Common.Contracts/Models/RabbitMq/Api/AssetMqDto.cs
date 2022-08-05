namespace AM.Services.Common.Contracts.Models.RabbitMq.Api;

public record AssetMqDto(string Id, byte TypeId, byte CountryId, string Name);