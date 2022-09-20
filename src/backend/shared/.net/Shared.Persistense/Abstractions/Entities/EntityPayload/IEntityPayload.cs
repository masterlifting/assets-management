namespace Shared.Persistense.Abstractions.Entities.EntityPayload;

public interface IEntityPayload : IEntity
{
    public string Name { get; init; }
    public string Source { get; init; }
    public byte[] Payload { get; init; }
    public int ContentTypeId { get; init; }
}
