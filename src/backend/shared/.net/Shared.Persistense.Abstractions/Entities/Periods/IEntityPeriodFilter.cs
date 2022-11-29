using System.Linq.Expressions;

namespace Shared.Persistense.Abstractions.Entities.PeriodsOfEntity;

public interface IEntityPeriodFilter<T> where T : class, IEntityPeriod, IPersistensable
{
    Expression<Func<T, bool>> Predicate { get; set; }
}