using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure.Persistense.Entities.EntityState;

public interface IEntityStateDbContext
{
    DbSet<StringId> StringIds { get; set; }
}
