using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Models.Api.Http;
using Shared.Contracts.Models.Pagination;
using Shared.Data;

namespace AM.Services.Portfolio.Core.Services.ControllersApi;

public class DealApi
{
    public Task<Paginated<DealGetDto>> GetAsync(string companyId, Paginator<Deal> paginator)
    {
        throw new NotImplementedException();
    }
    public Task<Paginated<DealGetDto>> GetAsync(Paginator<Deal> paginator)
    {
        throw new NotImplementedException();
    }
}