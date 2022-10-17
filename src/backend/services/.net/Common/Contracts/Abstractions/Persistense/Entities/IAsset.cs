using Shared.Persistense.Abstractions.Entities;

namespace AM.Services.Common.Contracts.Abstractions.Persistense.Entities;

public interface IAsset : IEntity
{
    string Id { get; init; }
    int TypeId { get; init; }
    int CountryId { get; set; }
    string Name { get; set; }
}