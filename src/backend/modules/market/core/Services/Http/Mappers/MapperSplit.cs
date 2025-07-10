﻿using AM.Services.Market.Domain.Entities;
using AM.Services.Market.Models.Api.Http;
using AM.Services.Market.Services.Http.Mappers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AM.Services.Market.Services.Http.Mappers;

public class MapperSplit : IMapperRead<Split, SplitGetDto>, IMapperWrite<Split, SplitPostDto>
{
    public async Task<SplitGetDto[]> MapFromAsync(IQueryable<Split> query) => await query
        .OrderByDescending(x => x.Date)
        .Select(x => new SplitGetDto
        {
            CompanyId = x.CompanyId,
            Company = x.Company.Name,
            Source = x.Source.Name,
            Date = x.Date,

            Value = x.Value
        })
        .ToArrayAsync();
    public async Task<SplitGetDto[]> MapLastFromAsync(IQueryable<Split> query)
    {
        var queryResult = await query
            .Select(x => new SplitGetDto
            {
                CompanyId = x.CompanyId,
                Company = x.Company.Name,
                Source = x.Source.Name,
                Date = x.Date,

                Value = x.Value
            })
            .ToArrayAsync();

        return queryResult
            .GroupBy(x => x.Company)
            .Select(x => x
                .OrderBy(y => y.Date)
                .Last())
            .OrderByDescending(x => x.Date)
            .ThenBy(x => x.Company)
            .ToArray();
    }

    public Split MapTo(Split id, SplitPostDto model) => new()
    {
        CompanyId = string.Intern(id.CompanyId.ToUpperInvariant()),
        SourceId = id.SourceId,
        Date = id.Date,

        Value = model.Value
    };
    public Split MapTo(string companyId, byte sourceId, SplitPostDto model) => new()
    {
        CompanyId = string.Intern(companyId),
        SourceId = sourceId,
        Date = model.Date,

        Value = model.Value
    };
    public Split[] MapTo(string companyId, byte sourceId, IEnumerable<SplitPostDto> models)
    {
        var dtos = models.ToArray();
        return dtos.Any()
            ? dtos.Select(x => MapTo(companyId, sourceId, x)).ToArray()
            : Array.Empty<Split>();
    }
}