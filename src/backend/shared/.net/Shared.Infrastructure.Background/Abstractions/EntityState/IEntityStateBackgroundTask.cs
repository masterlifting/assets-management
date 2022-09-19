using Shared.Infrastructure.Background.Settings;

namespace Shared.Infrastructure.Background.Abstractions.EntityState;

public interface IEntityStateBackgroundTask : IBackgroundTask
{
    Task StartAsync(BackgroundTaskSettings settings, CancellationToken cToken);
}