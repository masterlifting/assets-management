using Shared.Persistense.Entities.EntityState;

namespace Shared.Persistense.Abstractions.Entities.EntityState;

public interface IEntityState : IEntity
{
    string Id { get; init; }
    State State { get; set; }
    int StateId { get; set; }
    Step Step { get; init; }
    int StepId { get; set; }
    byte Attempt { get; set; }
}