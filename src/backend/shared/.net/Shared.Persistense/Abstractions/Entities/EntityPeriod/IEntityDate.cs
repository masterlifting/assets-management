namespace Shared.Persistense.Abstractions.Entities.EntityPeriod;

public interface IEntityDate : IEntityPeriod
{
    DateOnly Date { get; set; }
}