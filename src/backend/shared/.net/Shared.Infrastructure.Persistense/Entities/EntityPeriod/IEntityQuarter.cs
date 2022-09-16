namespace Shared.Infrastructure.Persistense.Entities.EntityPeriod;

public interface IEntityQuarter : IEntityPeriod
{
    int Year { get; set; }
    byte Quarter { get; set; }
}