namespace Shared.Persistense.Abstractions.Entities
{
    public interface IEntityCatalog : IEntity
    {
        int Id { get; init; }
        string Name { get; init; }
    }
}