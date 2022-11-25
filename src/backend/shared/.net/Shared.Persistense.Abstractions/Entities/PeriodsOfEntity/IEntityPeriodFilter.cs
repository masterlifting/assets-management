using System.Linq.Expressions;

namespace Shared.Persistense.Abstractions.Entities.PeriodsOfEntity;

public interface IEntityPeriodFilter<T> where T : class, IEntityPeriod, IEntity
{
    Expression<Func<T, bool>> Predicate { get; set; }
}