using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Persistense.Entities;

namespace Shared.Infrastructure.Persistense.Abstractions.Entities.State.Handle;

public interface IEntityStateDbContext
{
    DbSet<StringId> StringIds { get; set; }
}
