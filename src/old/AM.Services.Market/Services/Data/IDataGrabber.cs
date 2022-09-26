using AM.Services.Common.Contracts.Models.Entity.Interfaces;
using AM.Services.Market.Domain.Entities.Interfaces;
using AM.Services.Market.Domain.Entities.ManyToMany;

namespace AM.Services.Market.Services.Data;

public interface IDataGrabber<out TEntity> where TEntity : class, IDataIdentity, IPeriod
{
    IAsyncEnumerable<TEntity[]> GetHistoryDataAsync(CompanySource companySource);
    IAsyncEnumerable<TEntity[]> GetHistoryDataAsync(IEnumerable<CompanySource> companySources);

    IAsyncEnumerable<TEntity[]> GetCurrentDataAsync(CompanySource companySource);
    IAsyncEnumerable<TEntity[]> GetCurrentDataAsync(IEnumerable<CompanySource> companySources);
}