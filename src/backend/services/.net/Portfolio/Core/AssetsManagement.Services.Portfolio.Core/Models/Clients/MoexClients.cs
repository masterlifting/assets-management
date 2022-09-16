namespace AM.Services.Portfolio.Core.Models.Clients;

public record MoexIsinData(Securities Securities);
public record Securities(object[][] Data);