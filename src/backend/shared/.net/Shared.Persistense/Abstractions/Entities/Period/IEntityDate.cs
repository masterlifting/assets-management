namespace Shared.Persistense.Abstractions.Entities.Period;

public interface IEntityDate : IEntityPeriod
{
    DateOnly Date { get; set; }
}