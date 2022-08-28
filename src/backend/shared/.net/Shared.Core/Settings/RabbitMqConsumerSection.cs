using Shared.Core.Abstractions.Background.MessageQueue;
using Shared.Core.Domains.RabbitMq;

namespace Shared.Core.Settings;

public class RabbitMqConsumerSection : IMqConsumerSection<RabbitMqConsumerSettings>
{
    public const string SectionName = "RabbitMq";
    public Dictionary<string, RabbitMqConsumerSettings>? Consumers { get; set; }
}