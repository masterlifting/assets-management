using Shared.Core.Abstractions.Queues;

namespace Shared.Core.Domains.RabbitMq;

public class RabbitMqProducerMessageSettings : IMqProducerMessageSettings
{
    public int Version { get; set; }
}