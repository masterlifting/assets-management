using Shared.Core.Abstractions.Queues;

namespace Shared.Core.Abstractions.Background.MessageQueue;

public interface IMqConsumerSection<TSettings> where TSettings : class, IMqConsumerSettings
{
    public Dictionary<string, TSettings>? Consumers { get; set; }
}