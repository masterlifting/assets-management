namespace Shared.Persistense.Abstractions.Entities;

public interface IEntityDataAsByte : IEntityData
{
    byte[] Payload { get; init; }
    string PayloadSource { get; init; }
    string PayloadContentType { get; init; }
}
