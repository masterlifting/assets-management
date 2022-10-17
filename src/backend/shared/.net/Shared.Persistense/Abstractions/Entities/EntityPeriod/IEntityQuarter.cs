namespace Shared.Persistense.Abstractions.Entities.EntityPeriod;

public interface IEntityQuarter : IEntityPeriod
{
    int Year { get; set; }
    byte Quarter { get; set; }
}