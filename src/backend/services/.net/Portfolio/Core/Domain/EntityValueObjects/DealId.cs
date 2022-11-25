using Shared.Persistense.ValueObject.EntityState;

namespace AM.Services.Portfolio.Core.Domain.EntityValueObjects;

public sealed record DealId
{
    public string AsString { get; }
    public EntityStateId AsEntityStateId { get; }

    public DealId()
    {
        var entityStateId = new EntityStateId(Guid.NewGuid());
        AsString = entityStateId.Value;
        AsEntityStateId = entityStateId;
    }
}