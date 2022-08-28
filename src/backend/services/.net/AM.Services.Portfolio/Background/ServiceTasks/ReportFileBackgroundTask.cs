using AM.Services.Portfolio.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;

using Shared.Contracts.Settings;
using Shared.Core.Abstractions.Background.EntityState;
using Shared.Core.Background.EntityState;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using static AM.Services.Portfolio.Enums;

namespace AM.Services.Portfolio.Background.ServiceTasks;

public class ReportFileBackgroundTask : IEntityStateBackgroundTask<BackgroundTaskSettings>
{
    public string Name => "Reports";

    private readonly IServiceScopeFactory _scopeFactory;
    public ReportFileBackgroundTask(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public async Task StartAsync(BackgroundTaskSettings settings, CancellationToken cToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var task = scope.ServiceProvider.GetRequiredService<EntityStateTask<ReportFile>>();
        var handler = scope.ServiceProvider.GetRequiredService<IEntityStateBackgroundTaskHandler<ReportFile, BackgroundTaskSettings>>();

        var steps = new Queue<byte>(new[] { (byte)Steps.Parsing });

        await task.StartAsync(Name, steps, handler, settings, cToken);
    }
}