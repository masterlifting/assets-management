﻿using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Models.Api.Http;

using Shared.Contracts.Models.Pagination;

namespace AM.Services.Portfolio.Core.Services.ControllersApi;

public class DealApi
{
    public Task<PaginatedResult<DealGetDto>> GetAsync(string companyId, Paginator<Deal> paginator)
    {
        throw new NotImplementedException();
    }
    public Task<PaginatedResult<DealGetDto>> GetAsync(Paginator<Deal> paginator)
    {
        throw new NotImplementedException();
    }
}