namespace Shared.Persistense.Abstractions.Entities;

public interface IPersistensableJson : IPersistensable
{
    string Version { get; init; }
}
