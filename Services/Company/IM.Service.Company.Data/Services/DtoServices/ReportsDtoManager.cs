﻿using IM.Service.Common.Net.HttpServices;
using IM.Service.Common.Net.Models.Dto.Http;
using IM.Service.Common.Net.Models.Dto.Http.CompanyServices;
using IM.Service.Common.Net.RepositoryService.Filters;
using IM.Service.Company.Data.DataAccess.Entities;
using IM.Service.Company.Data.DataAccess.Repository;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IM.Service.Common.Net.RepositoryService.Comparators;

namespace IM.Service.Company.Data.Services.DtoServices;

public class ReportsDtoManager
{
    private readonly Repository<Report> reportRepository;
    private readonly Repository<DataAccess.Entities.Company> companyRepository;
    public ReportsDtoManager(
        Repository<Report> reportRepository,
        Repository<DataAccess.Entities.Company> companyRepository)
    {
        this.reportRepository = reportRepository;
        this.companyRepository = companyRepository;
    }

    public async Task<ResponseModel<ReportGetDto>> GetAsync(string companyId, int year, byte quarter)
    {
        companyId = companyId.ToUpperInvariant().Trim();
        var company = await companyRepository.FindAsync(companyId.ToUpperInvariant().Trim());

        if (company is null)
            return new() { Errors = new[] { $"'{companyId}' not found" } };

        var report = await reportRepository.FindAsync(company.Id, year, quarter);

        if (report is not null)
            return new()
            {
                Data = new()
                {
                    Ticker = company.Id,
                    Company = company.Name,
                    Year = report.Year,
                    Quarter = report.Quarter,
                    SourceType = report.SourceType,
                    Multiplier = report.Multiplier,
                    Asset = report.Asset,
                    CashFlow = report.CashFlow,
                    LongTermDebt = report.LongTermDebt,
                    Obligation = report.Obligation,
                    ProfitGross = report.ProfitGross,
                    ProfitNet = report.ProfitNet,
                    Revenue = report.Revenue,
                    ShareCapital = report.ShareCapital,
                    Turnover = report.Turnover
                }
            };

        return new()
        {
            Errors = new[] { $"Report for '{companyId}' not found" }
        };
    }
    public async Task<ResponseModel<PaginatedModel<ReportGetDto>>> GetAsync(CompanyDataFilterByQuarter<Report> filter, HttpPagination pagination)
    {
        var filteredQuery = reportRepository.GetQuery(filter.FilterExpression);
        var count = await reportRepository.GetCountAsync(filteredQuery);
        var paginatedQuery = reportRepository.GetPaginationQuery(filteredQuery, pagination, x => x.Year, x => x.Quarter);

        var result = await paginatedQuery
            .Join(companyRepository.GetDbSet(), x => x.CompanyId, y => y.Id, (x, y) => new ReportGetDto
            {
                Ticker = y.Id,
                Company = y.Name,
                Year = x.Year,
                Quarter = x.Quarter,
                SourceType = x.SourceType,
                Multiplier = x.Multiplier,
                Asset = x.Asset,
                CashFlow = x.CashFlow,
                LongTermDebt = x.LongTermDebt,
                Obligation = x.Obligation,
                ProfitGross = x.ProfitGross,
                ProfitNet = x.ProfitNet,
                Revenue = x.Revenue,
                ShareCapital = x.ShareCapital,
                Turnover = x.Turnover
            })
            .ToArrayAsync();

        return new()
        {
            Data = new()
            {
                Items = result,
                Count = count
            }
        };
    }
    public async Task<ResponseModel<PaginatedModel<ReportGetDto>>> GetLastAsync(CompanyDataFilterByQuarter<Report> filter, HttpPagination pagination)
    {
        var filteredQuery = reportRepository.GetQuery(filter.FilterExpression);

        var queryResult = await filteredQuery
            .Join(companyRepository.GetDbSet(), x => x.CompanyId, y => y.Id, (x, y) => new ReportGetDto
            {
                Ticker = y.Id,
                Company = y.Name,
                Year = x.Year,
                Quarter = x.Quarter,
                SourceType = x.SourceType,
                Multiplier = x.Multiplier,
                Asset = x.Asset,
                CashFlow = x.CashFlow,
                LongTermDebt = x.LongTermDebt,
                Obligation = x.Obligation,
                ProfitGross = x.ProfitGross,
                ProfitNet = x.ProfitNet,
                Revenue = x.Revenue,
                ShareCapital = x.ShareCapital,
                Turnover = x.Turnover
            })
            .ToArrayAsync();

        var groupedResult = queryResult
            .GroupBy(x => x.Company)
            .Select(x => x
                .OrderBy(y => y.Year)
                .ThenBy(y => y.Quarter)
                .Last())
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Quarter)
            .ThenBy(x => x.Company)
            .ToArray();

        return new()
        {
            Data = new()
            {
                Items = pagination.GetPaginatedResult(groupedResult),
                Count = groupedResult.Length
            }
        };
    }

    public async Task<ResponseModel<string>> CreateAsync(ReportPostDto model)
    {
        var ctxEntity = new Report
        {
            CompanyId = model.CompanyId,
            Year = model.Year,
            Quarter = model.Quarter,
            SourceType = model.SourceType,
            Multiplier = model.Multiplier,
            Asset = model.Asset,
            CashFlow = model.CashFlow,
            LongTermDebt = model.LongTermDebt,
            Obligation = model.Obligation,
            ProfitGross = model.ProfitGross,
            ProfitNet = model.ProfitNet,
            Revenue = model.Revenue,
            ShareCapital = model.ShareCapital,
            Turnover = model.Turnover
        };

        var message = $"Report of '{model.CompanyId}' create at {model.Year} - {model.Quarter}";

        var (error, _) = await reportRepository.CreateAsync(ctxEntity, message);

        return error is not null
            ? new() { Errors = new[] { error } }
            : new() { Data = message + " success" };
    }
    public async Task<ResponseModel<string>> CreateAsync(IEnumerable<ReportPostDto> models)
    {
        var reports = models.ToArray();

        if (!reports.Any())
            return new() { Errors = new[] { "Report data for creating not found" } };

        var ctxEntities = reports.GroupBy(x => (x.Year, x.Quarter)).Select(x => new Report
        {
            CompanyId = x.Last().CompanyId,
            Year = x.Last().Year,
            Quarter = x.Last().Quarter,
            SourceType = x.Last().SourceType,
            Multiplier = x.Last().Multiplier,
            Asset = x.Last().Asset,
            CashFlow = x.Last().CashFlow,
            LongTermDebt = x.Last().LongTermDebt,
            Obligation = x.Last().Obligation,
            ProfitGross = x.Last().ProfitGross,
            ProfitNet = x.Last().ProfitNet,
            Revenue = x.Last().Revenue,
            ShareCapital = x.Last().ShareCapital,
            Turnover = x.Last().Turnover
        });

        var (error, result) = await reportRepository.CreateAsync(ctxEntities, new CompanyQuarterComparer<Report>(), "Reports");

        return error is not null
            ? new() { Errors = new[] { error } }
            : new() { Data = $"Reports count: {result.Length} was successed" };
    }
    public async Task<ResponseModel<string>> UpdateAsync(ReportPostDto model)
    {
        var entity = new Report
        {
            CompanyId = model.CompanyId,
            Year = model.Year,
            Quarter = model.Quarter,
            SourceType = model.SourceType,
            Multiplier = model.Multiplier,
            Asset = model.Asset,
            CashFlow = model.CashFlow,
            LongTermDebt = model.LongTermDebt,
            Obligation = model.Obligation,
            ProfitGross = model.ProfitGross,
            ProfitNet = model.ProfitNet,
            Revenue = model.Revenue,
            ShareCapital = model.ShareCapital,
            Turnover = model.Turnover
        };

        var info = $"Report of '{model.CompanyId}' update at {model.Year} - {model.Quarter}";

        var (error, _) = await reportRepository.UpdateAsync(new object[] { entity.CompanyId, entity.Year, entity.Quarter }, entity, info );

        return error is not null
            ? new() { Errors = new[] { error } }
            : new() { Data = info + " success" };
    }
    public async Task<ResponseModel<string>> DeleteAsync(string companyId, int year, byte quarter)
    {
        companyId = companyId.ToUpperInvariant().Trim();
        var info = $"Report of '{companyId}' delete at {year} - {quarter}";

        var (error, _) = await reportRepository.DeleteAsync(new object[]{ companyId, year, quarter } , info );

        return error is not null
            ? new() { Errors = new[] { error } }
            : new() { Data = info + " success" };
    }
}