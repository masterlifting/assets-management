using System.Threading.Tasks;

namespace AM.Services.Common.Contracts.Background;

public interface IBackgroundTask
{
    Task StartAsync<T>(T settings) where T : BackgroundTaskSettings;
}