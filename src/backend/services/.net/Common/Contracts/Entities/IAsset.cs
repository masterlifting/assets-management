using Shared.Infrastructure.Persistense.Entities;

namespace AM.Services.Common.Contracts.Entities;

public interface IAsset : IEntity
{
    string Id { get; init; }
    int AssetTypeId { get; init; }
    int CountryId { get; set; }
    string Name { get; set; }
    string? Description { get; set; }
}