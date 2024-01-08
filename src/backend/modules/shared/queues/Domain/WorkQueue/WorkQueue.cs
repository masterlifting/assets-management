using System.Collections.Concurrent;

namespace Shared.Queue.Domain.WorkQueue;
/// <summary>
///  Asynchronously processing functions in the concurrent queue
/// </summary>
/// <remarks>Disposable</remarks>
public sealed class WorkQueue : IWorkQueue
{
    private record WorkQueueItem(Func<Task> Func, TaskCompletionSource TaskCompletionSource);
    private readonly BlockingCollection<WorkQueueItem> _queueItems;

    public WorkQueue()
    {
        _queueItems = new();
        Task.Run(ProcessQueueItemsAsync);
    }
    public WorkQueue(int itemsCount)
    {
        _queueItems = new(itemsCount);
        Task.Run(ProcessQueueItemsAsync);
    }

    public Task ProcessAsync(Func<Task> func)
    {
        TaskCompletionSource tcs = new();
        while (!_queueItems.TryAdd(new(func, tcs))) ;
        return tcs.Task;
    }
    public Task ProcessAsync(Func<Task>[] funcs)
    {
        List<Task> results = new(funcs.Length);

        foreach (var func in funcs)
        {
            TaskCompletionSource tcs = new();

            while (!_queueItems.TryAdd(new(func, tcs)))
                results.Add(tcs.Task);
        }

        return Task.WhenAll(results);
    }

    public void Dispose() => _queueItems.Dispose();

    private async Task ProcessQueueItemsAsync()
    {
        foreach (var item in _queueItems.GetConsumingEnumerable())
        {
            try
            {
                await item.Func.Invoke();
                item.TaskCompletionSource.SetResult();
            }
            catch (Exception exeption)
            {
                item.TaskCompletionSource.SetException(exeption);
            }
        }
    }
}
