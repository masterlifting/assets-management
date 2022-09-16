namespace Shared.Infrastructure.Persistense.Entities.EntityPeriod;

public interface IEntityTime : IEntityPeriod
{
    TimeOnly Time { get; set; }
}