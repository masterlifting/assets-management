﻿using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Enums;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shared.Persistense.Handlers;
using Shared.Background.Abstractions.EntityState;
using Shared.Background.Settings;

namespace AM.Services.Portfolio.Host.Services.Background.EntityState.Tasks;

public class DealBackgroundTask : IEntityStateBackgroundTask
{
    private readonly IServiceScopeFactory _scopeFactory;
    public string Name { get; }
    public DealBackgroundTask(string taskName, IServiceScopeFactory scopeFactory)
    {
        Name = taskName;
        _scopeFactory = scopeFactory;
    }
    public async Task StartAsync(BackgroundTaskSettings settings, CancellationToken cToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var pipeline = scope.ServiceProvider.GetRequiredService<EntityStatePipeline<Deal>>();

        var steps = new Queue<int>(new[]
        {
            (int)Steps.Calculating
        });

        await pipeline.StartAsync(settings, steps, cToken);
    }
}