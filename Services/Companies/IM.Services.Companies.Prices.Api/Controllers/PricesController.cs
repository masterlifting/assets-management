﻿using CommonServices.Models.Dto;
using CommonServices.Models.Dto.Http;

using IM.Services.Companies.Prices.Api.Services.DtoServices;

using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace IM.Services.Companies.Prices.Api.Controllers
{
    [ApiController, Route("[controller]")]
    public class PricesController : Controller
    {
        private readonly PriceDtoAggregator aggregator;
        public PricesController(PriceDtoAggregator aggregator) => this.aggregator = aggregator;

        public async Task<ResponseModel<PaginationResponseModel<PriceDto>>> Get(int year = 0, int month = 0, int day = 0, int page = 0, int limit = 0) =>
            await aggregator.GetPricesAsync(new(year, month, day), new(page, limit));

        [HttpGet("{ticker}")]
        public async Task<ResponseModel<PaginationResponseModel<PriceDto>>> Get(string ticker, int year = 0, int month = 0, int day = 0, int page = 0, int limit = 0) =>
            await aggregator.GetPricesAsync(ticker, new(year, month, day), new(page, limit));
    }
}
