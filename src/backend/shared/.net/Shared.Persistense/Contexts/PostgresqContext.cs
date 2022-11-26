using Microsoft.EntityFrameworkCore;

using Shared.Persistense.Abstractions.Entities;

namespace Shared.Persistense.Contexts
{
    public class PostgresqContext : DbContext
    {
        public PostgresqContext(DbContextOptions<PostgresqContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }
        public DbSet<GuidId> GuidIds { get; set; }
    }
}
