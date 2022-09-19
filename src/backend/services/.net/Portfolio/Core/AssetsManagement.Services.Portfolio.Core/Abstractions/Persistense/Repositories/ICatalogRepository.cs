using Shared.Persistense.Abstractions.Repositories;
using Shared.Persistense.Entities;

namespace AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;

public interface ICatalogRepository<T> : IRepository<T> where T : Catalog
{
    Task<T[]> GetAsync();
}