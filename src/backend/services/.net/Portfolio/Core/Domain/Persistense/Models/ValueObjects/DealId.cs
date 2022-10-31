using Shared.Persistense.Models.ValueObject.EntityState;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public sealed record DealId
{
    public string AsString { get; }
    public EntityStateId AsEntityStateId { get; }

    public DealId()
    {
        var entityStateId = new EntityStateId(Guid.NewGuid());
        AsString = entityStateId.AsString;
        AsEntityStateId = entityStateId;
    }
}
