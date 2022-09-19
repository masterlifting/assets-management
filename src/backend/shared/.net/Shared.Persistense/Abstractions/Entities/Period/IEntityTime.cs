namespace Shared.Persistense.Abstractions.Entities.Period;

public interface IEntityTime : IEntityPeriod
{
    TimeOnly Time { get; set; }
}