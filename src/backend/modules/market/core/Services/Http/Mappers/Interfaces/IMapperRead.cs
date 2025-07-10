﻿using AM.Services.Market.Domain.Entities.Interfaces;

namespace AM.Services.Market.Services.Http.Mappers.Interfaces;

public interface IMapperRead<in TEntity, TGet> where TGet : class where TEntity : class, IDataIdentity
{
    Task<TGet[]> MapFromAsync(IQueryable<TEntity> query);
    Task<TGet[]> MapLastFromAsync(IQueryable<TEntity> query);
}