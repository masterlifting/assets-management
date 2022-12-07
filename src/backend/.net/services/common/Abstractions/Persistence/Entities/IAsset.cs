namespace AM.Services.Common.Abstractions.Persistence.Entities;

public interface IAsset
{
    string Name { get; set; }
    int TypeId { get; init; }
    int CountryId { get; set; }
}