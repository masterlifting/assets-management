namespace AM.Services.Common.Abstractions.Persistence.Entities;

public interface IAsset
{
    string Name { get; init; }
    int TypeId { get; init; }
    int CountryId { get; init; }
}