﻿using AM.Services.Common.Contracts.Attributes;

namespace AM.Services.Market.Models.Api.Http;

public record FloatGetDto
{
    public string CompanyId { get; init; } = null!;
    public string Company { get; init; } = null!;
    public string Source { get; init; } = null!;
    public DateOnly Date { get; init; }
    public long Value { get; init; }
    public long? ValueFree { get; init; }
}
public record FloatPostDto : FloatPutDto
{
    public DateOnly Date { get; init; }
}
public record FloatPutDto
{
    [MoreZero(nameof(Value))]
    public long Value { get; init; }
    public long? ValueFree { get; init; }
}