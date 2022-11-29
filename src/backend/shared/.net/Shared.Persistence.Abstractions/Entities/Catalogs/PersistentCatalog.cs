using Shared.Persistence.Abstractions.Entities;

namespace Shared.Persistence.Abstractions.Entities.Catalogs;

public abstract class PersistentCatalog : IPersistentCatalog
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Info { get; set; }
    public DateTime Created { get; init; } = DateTime.UtcNow;
}