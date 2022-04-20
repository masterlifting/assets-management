﻿using IM.Service.Common.Net.Attributes;

namespace IM.Service.Market.Models.Api.Http;

public record PriceGetDto
{
    public string Company { get; init; } = null!;
    public string Source { get; init; } = null!;
    public DateOnly Date { get; init; }
    public string Currency { get; init; } = null!;
    public decimal Value { get; init; }
    public decimal ValueTrue { get; init; }
}
public record PricePostDto : PricePutDto
{
    public DateOnly Date { get; init; }
}
public record PricePutDto
{
    [MoreZero(nameof(CurrencyId))]
    public byte CurrencyId { get; init; }
    [MoreZero(nameof(Value))]
    public decimal Value { get; init; }
}