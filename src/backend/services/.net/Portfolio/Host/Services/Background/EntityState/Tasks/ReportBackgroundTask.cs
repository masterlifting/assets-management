﻿using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Enums;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shared.Persistense.Handlers;
using Shared.Background.Abstractions.EntityState;
using Shared.Background.Settings;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;
using AM.Services.Portfolio.Host.Exceptions;
using System.Linq;
using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;

namespace AM.Services.Portfolio.Host.Services.Background.EntityState.Tasks;

public class ReportBackgroundTask : IEntityStateBackgroundTask
{
    private readonly IServiceScopeFactory _scopeFactory;
    public string Name { get; }
    public ReportBackgroundTask(string taskName, IServiceScopeFactory scopeFactory)
    {
        Name = taskName;
        _scopeFactory = scopeFactory;
    }
    public async Task StartAsync(int count, BackgroundTaskSettings settings, CancellationToken cToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        
        var stepRepository = serviceProvider.GetRequiredService<ICatalogRepository<Step>>();
        var dbSteps = await stepRepository.GetAsync();
        var steps = SetQueueSteps(dbSteps);

        var pipeline = serviceProvider.GetRequiredService<EntityStatePipeline<Report>>();
        await pipeline.StartAsync(count, settings, steps, cToken);
    }
    private Queue<Step> SetQueueSteps(IReadOnlyCollection<Step> steps)
    {
        var result = new Queue<Step>(steps.Count);
        
        var calculatingStep = steps.FirstOrDefault(x => x.Id == (int) Steps.Parsing);
        
        if (calculatingStep is null)
            throw new PortfolioHostException(Name, $"Добавление в очередь шага обработки: {nameof(Steps.Parsing)}", "Отсутствует в базе данных");
        
        result.Enqueue(calculatingStep);
        return result;
    }
}