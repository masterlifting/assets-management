using AM.Services.Common.Contracts.Models.Entity.Interfaces;
using AM.Services.Common.Contracts.Models.Service;
using AM.Services.Common.Contracts.SqlAccess.Filters;
using AM.Services.Market.Domain.Entities.Interfaces;
using AM.Services.Market.Services.Http.Common.Interfaces;
using AM.Services.Market.Services.Http.Mappers.Interfaces;
using static AM.Services.Common.Contracts.Helpers.ServiceHelper;

namespace AM.Services.Market.Services.Http.Common;

public class RestApiRead<TEntity, TGet> where TGet : class where TEntity : class, IDataIdentity, IPeriod
{
    private readonly IRestQueryService<TEntity> queryService;
    private readonly IMapperRead<TEntity, TGet> mapper;

    public RestApiRead(IRestQueryService<TEntity> queryService, IMapperRead<TEntity, TGet> mapper)
    {
        this.queryService = queryService;
        this.mapper = mapper;
    }

    public async Task<PaginationModel<TGet>> GetAsync<T>(T filter, Paginatior pagination) where T : class, IFilter<TEntity>
    {
        var (query, count) = await queryService.GetQueryWithCountAsync(filter, pagination);

        return new PaginationModel<TGet> { Items = await mapper.MapFromAsync(query), Count = count };
    }
    public async Task<PaginationModel<TGet>> GetLastAsync<T>(T filter, Paginatior pagination) where T : class, IFilter<TEntity>
    {
        var query = queryService.GetQuery(filter);

        var lastResult = await mapper.MapLastFromAsync(query);

        return new PaginationModel<TGet> { Items = pagination.GetPaginatedResult(lastResult), Count = lastResult.Length };
    }
}
