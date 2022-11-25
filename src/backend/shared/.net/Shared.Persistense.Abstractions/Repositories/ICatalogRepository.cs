using Shared.Persistense.Abstractions.Entities;

namespace Shared.Persistense.Abstractions.Repositories;

public interface ICatalogRepository<T> : IEntityRepository<T> where T : class, IEntityCatalog
{
    Task<T[]> GetItemsAsync();
    Task<Dictionary<int, T>> GetDictionaryByIdAsync();
    Task<Dictionary<string, T>> GetDictionaryByNameAsync();
    ValueTask<T?> GetItemByIdAsync(int id);
    Task<T?> GetItemByNameAsync(string name);
}