namespace Shared.Infrastructure.Persistense.Abstractions.Entities.Period;

public interface IEntityTime : IEntityPeriod
{
    TimeOnly Time { get; set; }
}