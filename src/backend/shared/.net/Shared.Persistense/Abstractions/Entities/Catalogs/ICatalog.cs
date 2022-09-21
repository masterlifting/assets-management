namespace Shared.Persistense.Abstractions.Entities.Catalogs
{
    public interface ICatalog : IEntity
    {
        int Id { get; init; }
        string Name { get; init; }
    }
}