using Shared.Background.Settings;

namespace Shared.Background.Abstractions.EntityState;

public interface IEntityStateBackgroundTask : IBackgroundTask
{
    Task StartAsync(BackgroundTaskSettings settings, CancellationToken cToken);
}