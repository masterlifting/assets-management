using System.Linq.Expressions;

using Shared.Persistence.Abstractions.Entities;

using static Shared.Persistence.Abstractions.Constants.Enums;

namespace Shared.Persistence.Abstractions.Contexts
{
    public interface IMongoPersistenceContext : IPersistenceContext<IPersistentNoSql>
    {
        Task<uint> UpdateAsync<T>(Expression<Func<T, bool>> condition, Dictionary<ContextCommand, (string Name, string Value)> updater, CancellationToken cToken = default) where T : class, IPersistentNoSql;
    }
}
