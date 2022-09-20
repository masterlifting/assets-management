namespace Shared.Persistense.Abstractions.Entities.EntityState;

public interface IEntityState : IEntity
{
    string Id { get; init; }
    int StateId { get; set; }
    int StepId { get; set; }
    byte Attempt { get; set; }
}