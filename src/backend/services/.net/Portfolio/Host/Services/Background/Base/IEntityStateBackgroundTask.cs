using Shared.Contracts.Settings;

using System.Threading;
using System.Threading.Tasks;

namespace AM.Services.Portfolio.Host.Services.Background.Base;

public interface IEntityStateBackgroundTask
{
    string Name { get; }
    Task StartAsync(BackgroundTaskSettings settings, CancellationToken cToken);
}