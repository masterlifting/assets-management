using AM.Services.Common.Contracts.Models.Entity.Interfaces;
using AM.Services.Market.Domain.Entities.Interfaces;
using AM.Services.Market.Domain.Entities.ManyToMany;

namespace AM.Services.Market.Services.Data;

public interface ILastDataHelper<TEntity> where TEntity : class, IDataIdentity, IPeriod
{
    Task<TEntity?> GetLastDataAsync(CompanySource companySource);
    Task<TEntity[]> GetLastDataAsync(CompanySource[] companySources);
}