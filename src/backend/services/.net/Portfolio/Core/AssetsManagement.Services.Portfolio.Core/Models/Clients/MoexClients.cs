namespace AM.Services.Portfolio.Core.Models.Clients
{
    public sealed record MoexIsinData(Securities Securities);
    public sealed record Securities(object[][] Data);
}