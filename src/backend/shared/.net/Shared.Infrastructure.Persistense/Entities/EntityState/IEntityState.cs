namespace Shared.Infrastructure.Persistense.Entities.EntityState;

public interface IEntityState : IEntity
{
    public string Id { get; init; }
    public int StateId { get; set; }
    public int StepId { get; set; }
    public byte Attempt { get; set; }
    public string? Info { get; set; }
}