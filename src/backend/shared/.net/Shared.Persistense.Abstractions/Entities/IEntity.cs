namespace Shared.Persistense.Abstractions.Entities;

public interface IEntity
{
    DateTime Created { get; init; }
    string? Info { get; set; }
}