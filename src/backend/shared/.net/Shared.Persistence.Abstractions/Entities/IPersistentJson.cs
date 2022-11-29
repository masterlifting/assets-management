namespace Shared.Persistence.Abstractions.Entities;

public interface IPersistentJson : IPersistent
{
    string JsonVersion { get; init; }
}
