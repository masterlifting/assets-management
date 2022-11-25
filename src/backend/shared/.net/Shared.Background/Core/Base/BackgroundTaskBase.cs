using Microsoft.Extensions.Logging;
using Shared.Extensions.Logging;

using Shared.Background.Exceptions;
using Shared.Background.Settings;
using Shared.Persistense.Abstractions.Entities.Catalogs;
using Shared.Persistense.Abstractions.Repositories;

using System.Collections.Concurrent;

namespace Shared.Background.Core.Base
{
    public abstract class BackgroundTaskBase
    {
        private readonly ILogger _logger;
        private readonly ICatalogRepository<IProcessingStep> _stepCatalogRepository;

        protected BackgroundTaskBase(
            ILogger logger
            , ICatalogRepository<IProcessingStep> stepCatalogRepository)
        {
            _logger = logger;
            _stepCatalogRepository = stepCatalogRepository;
        }

        public async Task StartAsync(string taskName, int taskCount, BackgroundTaskSettings settings, CancellationToken cToken)
        {
            var steps = await GetQueueProcessStepsAsync(settings.Steps.Names);
            _logger.LogTrace(taskName, "Begin processing with the steps", "Start", "Steps count: " + steps.Count);


            if (settings.Steps.IsParallelProcessing)
                await ParallelHandleStepsAsync(new ConcurrentQueue<IProcessingStep>(steps), taskName, taskCount, settings, cToken);
            else
                await SuccessivelyHandleStepsAsync(steps, taskName, taskCount, settings, cToken);
        }

        internal async Task<Queue<IProcessingStep>> GetQueueProcessStepsAsync(string[] configurationSteps)
        {
            var result = new Queue<IProcessingStep>(configurationSteps.Length);
            var dbStepNames = await _stepCatalogRepository.GetDictionaryByNameAsync();

            foreach (var configurationStepName in configurationSteps)
                if (dbStepNames.ContainsKey(configurationStepName))
                    result.Enqueue(dbStepNames[configurationStepName]);
                else
                    throw new SharedBackgroundException(nameof(BackgroundTaskBase), nameof(GetQueueProcessStepsAsync), new($"Step from configuration: {configurationStepName} not found"));

            return result;
        }

        internal abstract Task SuccessivelyHandleStepsAsync(Queue<IProcessingStep> steps, string taskName, int taskCount, BackgroundTaskSettings settings, CancellationToken cToken);
        internal abstract Task ParallelHandleStepsAsync(ConcurrentQueue<IProcessingStep> steps, string taskName, int taskCount, BackgroundTaskSettings settings, CancellationToken cToken);
    }
}
