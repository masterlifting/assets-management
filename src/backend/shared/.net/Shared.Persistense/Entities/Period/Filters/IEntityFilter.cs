using System.Linq.Expressions;

using Shared.Persistense.Abstractions.Entities;

namespace Shared.Persistense.Entities.Period.Filters;

public interface IEntityFilter<T> where T : class, IEntity
{
    Expression<Func<T, bool>> Predicate { get; set; }
}