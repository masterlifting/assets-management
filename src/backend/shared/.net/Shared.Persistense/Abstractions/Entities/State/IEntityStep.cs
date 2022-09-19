namespace Shared.Persistense.Abstractions.Entities.State;

public interface IEntityStep : IEntity
{
    int Id { get; init; }
    string Name { get; init; }
    string? Description { get; set; }
}