namespace AM.Portfolio.Core.Models.Web.Http;

public sealed record MoexIsinData(Securities Securities);
public sealed record Securities(object[][] Data);
