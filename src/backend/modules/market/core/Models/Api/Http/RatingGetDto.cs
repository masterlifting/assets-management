﻿namespace AM.Services.Market.Models.Api.Http;

public record RatingGetDto
{
    public string CompanyId { get; init; } = null!;
    public string Company { get; init; } = null!;
    public int Place { get; set; }

    public decimal? Result { get; init; }
    public decimal? ResultPrice { get; init; }
    public decimal? ResultReport { get; init; }
    public decimal? ResultCoefficient { get; init; }
    public decimal? ResultDividend { get; init; }

    public DateTime UpdateTime { get; init; }
}