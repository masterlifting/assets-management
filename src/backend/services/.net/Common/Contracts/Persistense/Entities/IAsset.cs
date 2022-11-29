namespace AM.Services.Common.Contracts.Persistense.Entities;

public interface IAsset
{
    int TypeId { get; init; }
    int CountryId { get; set; }
    string Name { get; set; }
}