namespace Shared.Persistense.Abstractions.Entities.EntityState;

public interface IEntityStateCatalog : IEntity
{
    int Id { get; init; }
    string Name { get; init; }
    string? Description { get; set; }
}