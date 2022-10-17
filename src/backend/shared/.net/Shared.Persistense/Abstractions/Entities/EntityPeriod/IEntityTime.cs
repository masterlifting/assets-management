namespace Shared.Persistense.Abstractions.Entities.EntityPeriod;

public interface IEntityTime : IEntityPeriod
{
    TimeOnly Time { get; set; }
}