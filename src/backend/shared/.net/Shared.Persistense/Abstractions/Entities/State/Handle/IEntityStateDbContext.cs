using Microsoft.EntityFrameworkCore;

using Shared.Persistense.Entities;

namespace Shared.Persistense.Abstractions.Entities.State.Handle;

public interface IEntityStateDbContext
{
    DbSet<StringId> StringIds { get; set; }
}
