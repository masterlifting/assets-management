using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Shared.Persistense.Entities;
using Shared.Persistense.Repositories;

namespace AM.Services.Portfolio.Infrastructure.Persistence.Repositories
{
    public sealed class CatalogRepository<TCatalog, TContext> : Repository<TCatalog, TContext>
        where TContext : DbContext
        where TCatalog : Catalog
    {
        public CatalogRepository(ILogger<Catalog> logger, TContext context) : base(logger, context)
        {
        }

        public Task<TCatalog[]> GetAsync() => DbSet.ToArrayAsync();
    }
}