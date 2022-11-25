using Microsoft.EntityFrameworkCore;

using Shared.Persistense.Abstractions.Entities;

namespace Shared.Persistense.Abstractions.Context;

public interface IEntityStateDbContext
{
    DbSet<GuidId> GuidIds { get; set; }
}