using AM.Services.Common.Contracts.Helpers;
using AM.Services.Common.Contracts.Models.Entity.Interfaces;
using AM.Services.Common.Contracts.SqlAccess.Filters;
using AM.Services.Market.Domain.Entities.Interfaces;

namespace AM.Services.Market.Services.Http.Common.Interfaces;

public interface IRestQueryService<TEntity> where TEntity : class, IDataIdentity, IPeriod
{
    IQueryable<TEntity> GetQuery<T>(T filter) where T : class, IFilter<TEntity>;
    Task<(IQueryable<TEntity> query, int count)> GetQueryWithCountAsync<T>(T filter, ServiceHelper.Paginatior pagination) where T : class, IFilter<TEntity>;
}