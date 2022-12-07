using Microsoft.Extensions.Logging;
using Shared.Extensions.Logging;

using Shared.Background.Exceptions;
using Shared.Background.Settings;

using System.Collections.Concurrent;
using Shared.Persistence.Abstractions.Entities.Catalogs;
using Shared.Persistence.Abstractions.Repositories;

namespace Shared.Background.Core.Base;

public abstract class BackgroundTaskBase<TSTep> where TSTep : class, IProcessStep
{
    private readonly ILogger _logger;
    private readonly IPersistenceRepository _repository;

    protected BackgroundTaskBase(ILogger logger, IPersistenceRepository _repository)
    {
        _logger = logger;
        this._repository = _repository;
    }

    public async Task StartAsync(string taskName, int taskCount, BackgroundTaskSettings settings, CancellationToken cToken)
    {
        var steps = await GetQueueProcessStepsAsync(settings.Steps.Names);
        _logger.LogTrace(taskName, "Begin processing with the steps", "Start", "Steps count: " + steps.Count);


        if (settings.Steps.IsParallelProcessing)
            await ParallelHandleStepsAsync(new ConcurrentQueue<TSTep>(steps), taskName, taskCount, settings, cToken);
        else
            await SuccessivelyHandleStepsAsync(steps, taskName, taskCount, settings, cToken);
    }

    internal async Task<Queue<TSTep>> GetQueueProcessStepsAsync(string[] configurationSteps)
    {
        var result = new Queue<TSTep>(configurationSteps.Length);
        var dbStepNames = await _repository.GetCatalogsDictionaryByNameAsync<TSTep>();

        foreach (var configurationStepName in configurationSteps)
            if (dbStepNames.ContainsKey(configurationStepName))
                result.Enqueue(dbStepNames[configurationStepName]);
            else
                throw new SharedBackgroundException(nameof(BackgroundTaskBase<TSTep>), nameof(GetQueueProcessStepsAsync), new($"Step from configuration: {configurationStepName} not found"));

        return result;
    }

    internal abstract Task SuccessivelyHandleStepsAsync(Queue<TSTep> steps, string taskName, int taskCount, BackgroundTaskSettings settings, CancellationToken cToken);
    internal abstract Task ParallelHandleStepsAsync(ConcurrentQueue<TSTep> steps, string taskName, int taskCount, BackgroundTaskSettings settings, CancellationToken cToken);
}