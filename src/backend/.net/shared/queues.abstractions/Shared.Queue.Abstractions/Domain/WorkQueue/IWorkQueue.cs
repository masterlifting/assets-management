namespace Shared.Queue.Domain.WorkQueue
{
    /// <summary>
    ///  Asynchronously processing functions in the concurrent queue
    /// </summary>
    /// <remarks>Disposable</remarks>
    public interface IWorkQueue : IDisposable
    {
        Task ProcessAsync(Func<Task> func);
        Task ProcessAsync(Func<Task>[] funcs);
    }
}
