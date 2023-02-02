namespace Shared.Persistence.Abstractions.Contexts
{
    public interface IPersistenceContext : IDisposable
    {
        Task SetTransactionAsync(CancellationToken cToken);
        Task CommitTransactionAsync(CancellationToken cToken);
        Task RollbackTransactionAsync(CancellationToken cToken);
    }
}
