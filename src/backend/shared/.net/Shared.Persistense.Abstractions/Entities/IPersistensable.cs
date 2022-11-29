namespace Shared.Persistense.Abstractions.Entities;

public interface IPersistensable
{
    DateTime Created { get; init; }
    string? Info { get; set; }
}