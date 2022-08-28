namespace Shared.Core.Abstractions.Background.EntityState;

public interface IEntityStateBackgroundTask<in TSettings> : IBackgroundTask where TSettings : class
{
    Task StartAsync(TSettings settings, CancellationToken cToken);
}