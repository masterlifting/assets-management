namespace Shared.Persistense.Abstractions.Entities;

public interface IEntityDataAsByte : IEntityData
{
    byte[] Payload { get; init; }
    string PayloadSource { get; init; }
    int PayloadContentTypeId { get; init; }
}
