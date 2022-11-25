namespace Shared.Persistense.Abstractions.Entities.PeriodsOfEntity;

public interface IEntityTime : IEntityPeriod
{
    TimeOnly Time { get; set; }
}