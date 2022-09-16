namespace AM.Services.Portfolio.Core.Models.Api.Mq;

public record ProviderReportDto(string Name, string ContentType, byte[] Payload, string UserId);
