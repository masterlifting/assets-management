using System.Linq.Expressions;
using Shared.Infrastructure.Persistense.Abstractions.Entities;

namespace Shared.Infrastructure.Persistense.Entities.Period.Filters;

public interface IEntityFilter<T> where T : class, IEntity
{
    Expression<Func<T, bool>> Predicate { get; set; }
}