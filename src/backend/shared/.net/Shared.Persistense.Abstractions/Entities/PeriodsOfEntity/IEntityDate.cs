namespace Shared.Persistense.Abstractions.Entities.PeriodsOfEntity;

public interface IEntityDate : IEntityPeriod
{
    DateOnly Date { get; set; }
}