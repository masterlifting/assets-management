using System;
using System.Threading.Tasks;
using AM.Services.Common.Contracts.Models.Service;
using AM.Services.Portfolio.Models.Api.Http;
using static AM.Services.Common.Contracts.Helpers.ServiceHelper;

namespace AM.Services.Portfolio.Services.Http;

public class DealApi
{
    public Task<PaginationModel<DealGetDto>> GetAsync(string companyId, Paginatior paginatior)
    {
        throw new NotImplementedException();
    }
    public Task<PaginationModel<DealGetDto>> GetAsync(Paginatior paginatior)
    {
        throw new NotImplementedException();
    }
}