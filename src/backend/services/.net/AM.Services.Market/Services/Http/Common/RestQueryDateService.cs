using AM.Services.Common.Contracts.Models.Entity.Interfaces;
using AM.Services.Common.Contracts.SqlAccess.Filters;
using AM.Services.Market.Domain.DataAccess;
using AM.Services.Market.Domain.Entities.Interfaces;
using AM.Services.Market.Services.Http.Common.Interfaces;
using static AM.Services.Common.Contracts.Helpers.ServiceHelper;

namespace AM.Services.Market.Services.Http.Common;

public class RestQueryDateService<TEntity> : IRestQueryService<TEntity> where TEntity : class, IDataIdentity, IDateIdentity
{
    private readonly Repository<TEntity> repository;
    public RestQueryDateService(Repository<TEntity> repository) => this.repository = repository;

    public IQueryable<TEntity> GetQuery<T>(T filter) where T : class, IFilter<TEntity>
    {
        return repository.Where(filter.Expression);
    }
    public async Task<(IQueryable<TEntity> query, int count)> GetQueryWithCountAsync<T>(T filter, Paginatior pagination) where T : class, IFilter<TEntity>
    {
        var filteredQuery = repository.Where(filter.Expression);
        var count = await repository.GetCountAsync(filteredQuery);
        return (repository.GetPaginationQueryDesc(filteredQuery, pagination, x => x.Date), count);
    }
}