namespace Shared.Persistense.Abstractions.Entities;

public interface IPersistensableCatalog : IPersistensable
{
    int Id { get; init; }
    string Name { get; init; }
}