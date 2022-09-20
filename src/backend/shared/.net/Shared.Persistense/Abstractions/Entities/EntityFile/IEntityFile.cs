namespace Shared.Persistense.Abstractions.Entities.EntityFile;

public interface IEntityFile : IEntity
{
    string Name { get; init; }
    string Source { get; init; }
    byte[] Payload { get; init; }
    int ContentTypeId { get; init; }
}
