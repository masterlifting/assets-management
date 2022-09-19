namespace Shared.Persistense.Abstractions.Entities.Period;

public interface IEntityQuarter : IEntityPeriod
{
    int Year { get; set; }
    byte Quarter { get; set; }
}