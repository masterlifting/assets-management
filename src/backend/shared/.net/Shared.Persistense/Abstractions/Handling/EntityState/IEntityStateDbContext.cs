using Microsoft.EntityFrameworkCore;

using Shared.Persistense.Entities;

namespace Shared.Persistense.Abstractions.Handling.EntityState;
public interface IEntityStateDbContext
{
    DbSet<StringId> StringIds { get; set; }
}