﻿using CommonServices.Models.Dto.Http;

using IM.Gateway.Companies.DataAccess.Entities;
using IM.Gateway.Companies.DataAccess.Repository;
using IM.Gateway.Companies.Models.Dto;

using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Threading.Tasks;
using CommonServices.Models.Dto;

namespace IM.Gateway.Companies.Services.DtoServices
{
    public class StockSplitDtoAggregator
    {
        private readonly RepositorySet<StockSplit> repository;
        public StockSplitDtoAggregator(RepositorySet<StockSplit> repository) => this.repository = repository;

        public async Task<ResponseModel<PaginationResponseModel<StockSplitDto>>> GetAsync(FilterRequestModel filter, PaginationRequestModel pagination)
        {
            var errors = Array.Empty<string>();

            var count = await repository.GetCountAsync();

            var filteredQuery = repository.QueryFilter(filter.FilterDateExpression<StockSplit>());
            var paginatedQuery = repository.QueryPaginator(filteredQuery, pagination, x => x.Date);

            var companies = repository.GetDbSetBy<Company>();

            var result = await paginatedQuery.Join(companies, x => x.CompanyTicker, y => y.Ticker, (x, y) => new StockSplitDto
            {
                Id = x.Id,
                Company = y.Name,
                Ticker = x.CompanyTicker,
                Divider = x.Divider,
                Date = x.Date
            })
            .ToArrayAsync();

            return new()
            {
                Errors = errors,
                Data = new()
                {
                    Items = result,
                    Count = count
                }
            };
        }
        public async Task<ResponseModel<PaginationResponseModel<StockSplitDto>>> GetAsync(string ticker, FilterRequestModel filter, PaginationRequestModel pagination)
        {
            var companies = repository.GetDbSetBy<Company>();
            var company = await companies.FindAsync(ticker.ToUpperInvariant());

            if (company is null)
                return new() { Errors = new[] { "model not found" } };

            var errors = Array.Empty<string>();

            var count = await repository.GetCountAsync(x => x.CompanyTicker == company.Ticker);

            var dateFilteredQuery = repository.QueryFilter(filter.FilterDateExpression<StockSplit>());
            var resultFilteredQuery = repository.QueryFilter(dateFilteredQuery, x => x.CompanyTicker == company.Ticker);
            
            var paginatedResult = repository.QueryPaginator(resultFilteredQuery, pagination, x => x.Date);

            var result = await paginatedResult.Join(companies, x => x.CompanyTicker, y => y.Ticker, (x, y) => new StockSplitDto
            {
                Id = x.Id,
                Company = y.Name,
                Ticker = x.CompanyTicker,
                Divider = x.Divider,
                Date = x.Date
            })
                .ToArrayAsync();

            return new()
            {
                Errors = errors,
                Data = new()
                {
                    Items = result,
                    Count = count
                }
            };
        }
        public async Task<ResponseModel<string>> CreateAsync(StockSplitPostDto model)
        {
            var ctxEntity = new StockSplit()
            {
                CompanyTicker = model.Ticker,
                Divider = model.Divider,
                Date = model.Date
            };

            var (errors, createdEntity) = await repository.CreateAsync(ctxEntity, $"stock split for: '{model.Ticker}'");

            return errors.Any()
                ? new ResponseModel<string> { Errors = errors }
                : new ResponseModel<string> { Data = $"stock split for: '{createdEntity!.CompanyTicker}' created" };
        }
        public async Task<ResponseModel<string>> UpdateAsync(int id, StockSplitPostDto model)
        {
            var ctxEntity = new StockSplit()
            {
                Id = id,
                CompanyTicker = model.Ticker,
                Divider = model.Divider,
                Date = model.Date
            };

            var (errors, updatedEntity) = await repository.UpdateAsync(ctxEntity, $"stock split for: '{model.Ticker}'");

            return errors.Any()
                ? new ResponseModel<string> { Errors = errors }
                : new ResponseModel<string> { Data = $"stock split for: '{updatedEntity!.CompanyTicker}' updated" };
        }
        public async Task<ResponseModel<string>> DeleteAsync(int id)
        {
            var errors = await repository.DeleteAsync(id, $"stock split for id: '{id}'");

            return errors.Any()
                ? new ResponseModel<string> { Errors = errors }
                : new ResponseModel<string> { Data = $"stock split for id: '{id}' deleted" };
        }
    }
}
