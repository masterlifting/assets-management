using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;
using AM.Services.Portfolio.Host.Exceptions;
using AM.Services.Portfolio.Infrastructure.Persistence.Context;
using AM.Services.Portfolio.Infrastructure.Persistence.Repositories;

using Microsoft.Extensions.DependencyInjection;

using Shared.Background.Abstractions.Tasks;
using Shared.Background.Settings;
using Shared.Persistense.Entities.EntityState;
using Shared.Persistense.Handling.EntityState;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static Shared.Persistense.Constants.Enums;

namespace AM.Services.Portfolio.Host.Services.Background.EntityState.Tasks
{
    public sealed class ReportFileBackgroundTask : IEntityStateBackgroundTask
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public string Name { get; }
        public ReportFileBackgroundTask(string taskName, IServiceScopeFactory scopeFactory)
        {
            Name = taskName;
            _scopeFactory = scopeFactory;
        }
        public async Task StartAsync(int count, BackgroundTaskSettings settings, CancellationToken cToken)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var serviceProvider = scope.ServiceProvider;

            var stepRepository = serviceProvider.GetRequiredService<CatalogRepository<Step, DatabaseContext>>(); 
            var dbSteps = await stepRepository.GetAsync();
            var steps = SetQueueSteps(dbSteps);

            var pipeline = serviceProvider.GetRequiredService<EntityStateHandling<ReportData, IReportDataRepository>>();
            await pipeline.StartAsync(count, settings, steps, cToken);
        }
        private Queue<Step> SetQueueSteps(IReadOnlyCollection<Step> steps)
        {
            var result = new Queue<Step>(steps.Count);

            var calculatingStep = steps.FirstOrDefault(x => x.Id == (int)Steps.Parsing);

            if (calculatingStep is null)
                throw new PortfolioHostException(Name, $"Добавление в очередь шага обработки: {nameof(Steps.Parsing)}", "Отсутствует в базе данных");

            result.Enqueue(calculatingStep);
            return result;
        }
    }
}