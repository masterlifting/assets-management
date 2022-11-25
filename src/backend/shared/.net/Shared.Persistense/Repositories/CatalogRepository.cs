using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Abstractions.Entities.Catalogs;
using Shared.Persistense.Abstractions.Repositories;

namespace Shared.Persistense.Repositories;

public class CatalogRepository<TCatalog, TContext> : EntityRepository<TCatalog, TContext>, ICatalogRepository<TCatalog>
    where TContext : DbContext
    where TCatalog : class, IEntityCatalog
{
    private readonly TContext _context;
    public CatalogRepository(ILogger<Catalog> logger, TContext context) : base(logger, context) => _context = context;

    public Task<Dictionary<int, TCatalog>> GetDictionaryByIdAsync() => _context.Set<TCatalog>().ToDictionaryAsync(x => x.Id);
    public Task<Dictionary<string, TCatalog>> GetDictionaryByNameAsync() => _context.Set<TCatalog>().ToDictionaryAsync(x => x.Name);
    public ValueTask<TCatalog?> GetItemByIdAsync(int id) => _context.Set<TCatalog>().FindAsync(id);
    public Task<TCatalog?> GetItemByNameAsync(string name) => _context.Set<TCatalog>().FirstOrDefaultAsync(x => x.Name.Equals(name)));
    public Task<TCatalog[]> GetItemsAsync() => _context.Set<TCatalog>().ToArrayAsync();
}