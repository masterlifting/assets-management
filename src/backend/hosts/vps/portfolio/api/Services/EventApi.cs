using AM.Services.Portfolio.API.Models;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;
using Shared.Models.Pagination;
using Shared.Models.Results;
using System;
using System.Threading.Tasks;

namespace AM.Services.Portfolio.API.Services;

public sealed class EventApi
{
    public Task<PaginatedResult<EventGetDto>> GetAsync(string companyId, Paginator<Event> paginator)
    {
        throw new NotImplementedException();
    }
    public Task<PaginatedResult<EventGetDto>> GetAsync(Paginator<Event> paginator)
    {
        throw new NotImplementedException();
    }
}