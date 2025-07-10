namespace AM.Shared.Abstractions.Persistence.Entities;

public interface IAsset
{
    string Name { get; init; }
    int TypeId { get; init; }
    string? Label { get; init; }
}