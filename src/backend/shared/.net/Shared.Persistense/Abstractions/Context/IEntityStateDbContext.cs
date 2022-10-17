using Microsoft.EntityFrameworkCore;

using Shared.Persistense.Entities;

namespace Shared.Persistense.Abstractions.Context;

public interface IEntityStateDbContext
{
    DbSet<StringId> StringIds { get; set; }
}