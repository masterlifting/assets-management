namespace Shared.Infrastructure.Persistense.Entities.EntityPeriod;

public interface IEntityDate : IEntityPeriod
{
    DateOnly Date { get; set; }
}