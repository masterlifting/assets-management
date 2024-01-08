using AM.Services.Portfolio.API.Models;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using Shared.Models.Pagination;
using Shared.Models.Results;
using System;
using System.Threading.Tasks;

namespace AM.Services.Portfolio.API.Services;

public sealed class DealApi
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