using AM.Services.Portfolio.Core.Domain.Persistense.Entities;
using AM.Services.Portfolio.Core.Models.Api.Http;

using Shared.Contracts.Models.Pagination;

namespace AM.Services.Portfolio.Core.Services.ControllersApi;

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