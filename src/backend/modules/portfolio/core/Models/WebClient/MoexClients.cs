namespace AM.Services.Portfolio.Core.Models.WebClient;

public sealed record MoexIsinData(Securities Securities);
public sealed record Securities(object[][] Data);