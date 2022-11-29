namespace Shared.Persistense.Abstractions.Entities.Catalogs;

public abstract class PersistensableCatalog : IPersistensableCatalog
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Info { get; set; }
    public DateTime Created { get; init; } = DateTime.UtcNow;
}