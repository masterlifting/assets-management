using Shared.Persistense.Abstractions.Entities.EntityState;
using Shared.Persistense.Models.ValueObject.EntityState;

namespace Shared.Persistense.Entities.EntityState;

public sealed class State : Catalog, IEntityStateType
{
    public State()
    {
    }

    public State(StateId stateId) : base(stateId.AsInt, stateId.AsString)
    {
    }
}