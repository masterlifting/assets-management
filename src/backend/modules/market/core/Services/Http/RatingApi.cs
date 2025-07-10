﻿using System.Linq.Expressions;
using AM.Services.Common.Contracts.Models.Service;
using AM.Services.Market.Domain.DataAccess;
using AM.Services.Market.Domain.Entities;
using AM.Services.Market.Models.Api.Http;
using AM.Services.Market.Services.Entity;
using Microsoft.EntityFrameworkCore;
using static AM.Services.Common.Contracts.Enums;
using static AM.Services.Common.Contracts.Helpers.ServiceHelper;

namespace AM.Services.Market.Services.Http;

public class RatingApi
{
    private readonly Repository<Rating> ratingRepo;
    private readonly Repository<Company> companyRepo;
    private readonly RatingService ratingService;
    private const int resultRoundValue = 3;

    public RatingApi(
        Repository<Rating> ratingRepo,
        Repository<Company> companyRepo,
        RatingService ratingService)
    {
        this.ratingRepo = ratingRepo;
        this.companyRepo = companyRepo;
        this.ratingService = ratingService;
    }

    public async Task<RatingGetDto> GetAsync(int place)
    {
        var rating = await ratingRepo.DbSet
            .OrderByDescending(x => x.Result)
            .Skip(place - 1)
            .Take(1)
            .Select(x => new
            {
                CompanyName = x.Company.Name,
                x.Result,
                x.ResultPrice,
                x.ResultReport,
                x.ResultCoefficient,
                x.ResultDividend,
                x.Date,
                x.Time
            })
            .FirstOrDefaultAsync();

        return rating is null
            ? throw new NullReferenceException(nameof(rating))
            : new RatingGetDto
            {
                Place = place,
                Company = rating.CompanyName,
                Result = rating.Result.HasValue ? decimal.Round(rating.Result.Value, resultRoundValue) : null,
                ResultPrice = rating.ResultPrice.HasValue ? decimal.Round(rating.ResultPrice.Value, resultRoundValue) : null,
                ResultReport = rating.ResultReport.HasValue ? decimal.Round(rating.ResultReport.Value, resultRoundValue) : null,
                ResultCoefficient = rating.ResultCoefficient.HasValue ? decimal.Round(rating.ResultCoefficient.Value, resultRoundValue) : null,
                ResultDividend = rating.ResultDividend.HasValue ? decimal.Round(rating.ResultDividend.Value, resultRoundValue) : null,
                UpdateTime = new DateTime(rating.Date.Year, rating.Date.Month, rating.Date.Day, rating.Time.Hour, rating.Time.Minute, rating.Time.Second)
            };
    }
    public async Task<RatingGetDto> GetAsync(string companyId)
    {
        companyId = companyId.Trim().ToUpperInvariant();

        var company = await companyRepo.FindAsync(companyId);

        if (company is null)
            throw new NullReferenceException($"company by companyId: {companyId} not found");

        var orderedRatingIds = await ratingRepo.DbSet
            .OrderByDescending(x => x.Result)
            .Select(x => x.CompanyId)
            .ToArrayAsync();

        var places = orderedRatingIds
            .Select((x, i) => new { Place = i + 1, CompanyId = x })
            .ToDictionary(x => x.CompanyId, y => y.Place);

        var rating = await ratingRepo.FindAsync(x => x.CompanyId == company.Id);

        return rating is null
            ? throw new NullReferenceException(nameof(rating))
            : new RatingGetDto
            {
                CompanyId = company.Id,
                Company = company.Name,
                Place = places[rating.CompanyId],
                Result = rating.Result.HasValue ? decimal.Round(rating.Result.Value, resultRoundValue) : null,
                ResultPrice = rating.ResultPrice.HasValue ? decimal.Round(rating.ResultPrice.Value, resultRoundValue) : null,
                ResultReport = rating.ResultReport.HasValue ? decimal.Round(rating.ResultReport.Value, resultRoundValue) : null,
                ResultCoefficient = rating.ResultCoefficient.HasValue ? decimal.Round(rating.ResultCoefficient.Value, resultRoundValue) : null,
                ResultDividend = rating.ResultDividend.HasValue ? decimal.Round(rating.ResultDividend.Value, resultRoundValue) : null,
                UpdateTime = new DateTime(rating.Date.Year, rating.Date.Month, rating.Date.Day, rating.Time.Hour, rating.Time.Minute, rating.Time.Second)
            };
    }
    public async Task<PaginationModel<RatingGetDto>> GetAsync(Paginatior pagination, string entity)
    {
        var placeStart = (pagination.Page - 1) * pagination.Limit + 1;

        Expression<Func<Rating, decimal?>> orderSelector = entity switch
        {
            nameof(Rating) => x => x.Result,
            nameof(Price) => x => x.ResultPrice,
            nameof(Report) => x => x.ResultReport,
            nameof(Coefficient) => x => x.ResultCoefficient,
            nameof(Dividend) => x => x.ResultDividend,
            _ => throw new ArgumentOutOfRangeException(nameof(entity), entity, null)
        };

        var count = await ratingRepo.GetCountAsync();
        var data = await ratingRepo
            .GetPaginationQueryDesc(pagination, orderSelector)
            .Select(x => new
            {
                x.CompanyId,
                CompanyName = x.Company.Name,
                x.Result,
                x.ResultPrice,
                x.ResultReport,
                x.ResultCoefficient,
                x.ResultDividend,
                x.Date,
                x.Time
            })
            .ToArrayAsync();

        var result = data
            .Select((x, i) => new RatingGetDto
            {
                Place = placeStart + i,
                CompanyId = x.CompanyId,
                Company = x.CompanyName,
                Result = x.Result.HasValue ? decimal.Round(x.Result.Value, resultRoundValue) : null,
                ResultPrice = x.ResultPrice.HasValue ? decimal.Round(x.ResultPrice.Value, resultRoundValue) : null,
                ResultReport = x.ResultReport.HasValue ? decimal.Round(x.ResultReport.Value, resultRoundValue) : null,
                ResultCoefficient = x.ResultCoefficient.HasValue ? decimal.Round(x.ResultCoefficient.Value, resultRoundValue) : null,
                ResultDividend = x.ResultDividend.HasValue ? decimal.Round(x.ResultDividend.Value, resultRoundValue) : null,
                UpdateTime = new DateTime(x.Date.Year, x.Date.Month, x.Date.Day, x.Time.Hour, x.Time.Minute, x.Time.Second)
            })
            .ToArray();

        return new PaginationModel<RatingGetDto> { Items = result, Count = count };
    }

    public Task<string> RecalculateAsync(CompareType compareType, string? companyId, int year = 0, int month = 0, int day = 0) =>
        ratingService.RecompareAsync(compareType, companyId, year, month, day);
}