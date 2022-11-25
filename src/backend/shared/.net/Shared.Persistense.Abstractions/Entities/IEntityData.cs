namespace Shared.Persistense.Abstractions.Entities;

public interface IEntityData : IEntityProcessable
{
    Guid UserId { get; init; }
    byte[] SHA256Hash { get; init; }
}
