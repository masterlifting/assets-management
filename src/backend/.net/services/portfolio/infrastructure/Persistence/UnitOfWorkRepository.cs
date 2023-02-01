using AM.Services.Portfolio.Core.Abstractions.Persistence;
using AM.Services.Portfolio.Core.Abstractions.Persistence.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities.Catalogs;
using AM.Services.Portfolio.Infrastructure.Exceptions;

using Shared.Persistence.Abstractions.Repositories;

using System.Collections.Concurrent;

namespace AM.Services.Portfolio.Infrastructure.Persistence
{
    public class UnitOfWorkRepository : IUnitOfWorkRepository, IDisposable
    {
        private readonly WorkQueue _workQueue = new();
        public UnitOfWorkRepository(
            IPersistenceSqlRepository<ProcessStep> processStep
            , IIncomingDataRepository incomingData
            , IAssetRepository asset
            , IDealRepository deal
            , IEventRepository _event
            , IDerivativeRepository derivative
            , IUserRepository user
            , IAccountRepository account)
        {
            ProcessStep = processStep;
            IncomingData = incomingData;
            Asset = asset;
            Deal = deal;
            Event = _event;
            Derivative = derivative;
            User = user;
            Account = account;
        }

        public IPersistenceSqlRepository<ProcessStep> ProcessStep { get; }
        public IIncomingDataRepository IncomingData { get; }
        public IAssetRepository Asset { get; }
        public IDealRepository Deal { get; }
        public IEventRepository Event { get; }
        public IDerivativeRepository Derivative { get; }
        public IUserRepository User { get; }
        public IAccountRepository Account { get; }


        public async Task ProcessQueueAsync(params Task[] tasks)
        {
            foreach (var task in tasks)
            {
                try
                {
                    await _workQueue.ProcessAsync(task);
                }
                catch (Exception exeption)
                {
                    Console.WriteLine(exeption.InnerException?.Message ?? exeption.Message);
                    //throw new PortfolioInfrastructureException(nameof(UnitOfWorkRepository), nameof(ProcessQueueAsync), new(exeption));
                }
            }
        }
        public void Dispose() => _workQueue.Dispose();
    }
    sealed class WorkQueue : IDisposable
    {
        record WorkQueueItem(Task Task, TaskCompletionSource TaskCompletionSource);
        private readonly BlockingCollection<WorkQueueItem> _workQueueItems;
        public WorkQueue()
        {
            _workQueueItems = new();
            Task.Run(ProcessWorkItems);
        }

        public Task ProcessAsync(Task task)
        {
            var tcs = new TaskCompletionSource();
            while (!_workQueueItems.TryAdd(new WorkQueueItem(task, tcs))) ;
            return tcs.Task;
        }

        async Task ProcessWorkItems()
        {
            foreach (var item in _workQueueItems.GetConsumingEnumerable())
            {
                try
                {
                    await item.Task;
                    item.TaskCompletionSource.SetResult();
                }
                catch (Exception exeption)
                {
                    item.TaskCompletionSource.SetException(exeption);
                }
            }
        }

        public void Dispose() => _workQueueItems.Dispose();
    }
}
