﻿using IM.Service.Common.Net.Attributes;

namespace IM.Service.MarketData.Models.Api.Http;

public record SplitGetDto
{
    public string Company { get; init; } = null!;
    public string Source { get; init; } = null!;
    public DateOnly Date { get; init; }
    public int Value { get; init; }
}
public class SplitPostDto : SplitPutDto
{
    public string CompanyId { get; init; } = null!;
    public byte SourceId { get; init; }
    public DateOnly Date { get; set; }
}
public class SplitPutDto
{
    [NotZero(nameof(Value))]
    public int Value { get; set; }
}