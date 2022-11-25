namespace Shared.Persistense.Abstractions.Entities.PeriodsOfEntity;

public interface IEntityQuarter : IEntityPeriod
{
    int Year { get; set; }
    byte Quarter { get; set; }
}