using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Models.Api.Http;
using Shared.Contracts.Dto;
using Shared.Infrastructure.Data;

namespace AM.Services.Portfolio.Core.Services.ControllersApi;

public class EventApi
{
    public Task<Paginated<EventGetDto>> GetAsync(string companyId, Paginator<Event> paginator)
    {
        throw new NotImplementedException();
    }
    public Task<Paginated<EventGetDto>> GetAsync(Paginator<Event> paginator)
    {
        throw new NotImplementedException();
    }
}