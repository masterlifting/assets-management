using Shared.Persistence.Abstractions.Entities;

namespace Shared.Persistence.Abstractions.Contexts
{
    public interface IPostgrePersistenceContext : IPersistenceContext<IPersistentSql>
    {
    }
}
