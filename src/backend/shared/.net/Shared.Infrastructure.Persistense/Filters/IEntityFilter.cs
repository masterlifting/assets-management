using System.Linq.Expressions;
using Shared.Infrastructure.Persistense.Entities;

namespace Shared.Infrastructure.Persistense.Filters;

public interface IEntityFilter<T> where T : class, IEntity
{
    Expression<Func<T, bool>> Predicate { get; set; }
}