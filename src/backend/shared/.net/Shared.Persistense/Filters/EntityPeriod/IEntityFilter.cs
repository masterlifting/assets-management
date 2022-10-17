using System.Linq.Expressions;

using Shared.Persistense.Abstractions.Entities;

namespace Shared.Persistense.Filters.EntityPeriod;

public interface IEntityFilter<T> where T : class, IEntity
{
    Expression<Func<T, bool>> Predicate { get; set; }
}