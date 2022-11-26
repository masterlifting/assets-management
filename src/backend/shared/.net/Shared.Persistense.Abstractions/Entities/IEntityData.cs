namespace Shared.Persistense.Abstractions.Entities;

public interface IEntityData : IProcessableEntity
{
    Guid UserId { get; init; }
    byte[] SHA256Hash { get; init; }
}
