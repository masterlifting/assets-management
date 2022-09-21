namespace AM.Services.Portfolio.Core.Models.Api.Mq
{
    public sealed record ProviderReportDto(string Name, string ContentType, byte[] Payload, string UserId);
}