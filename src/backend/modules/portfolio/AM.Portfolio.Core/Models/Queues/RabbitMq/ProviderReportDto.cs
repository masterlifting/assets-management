namespace AM.Portfolio.Core.Models.Queues.RabbitMq;

public sealed record ProviderReportDto(string Name, string ContentType, byte[] Payload, string UserId);